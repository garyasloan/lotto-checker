import { useEffect, useRef, useState } from 'react';
import { toast } from 'react-hot-toast';
import {
  DataGrid,
  type GridColDef,
  type GridRenderCellParams,
  GridFooterContainer,
  GridPagination,
} from '@mui/x-data-grid';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
  Typography,
  Card,
  useMediaQuery,
  SwipeableDrawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Divider,
  Tooltip
} from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useQueryClient } from '@tanstack/react-query';
import { useLottoPicks } from '../../../hooks/useLottoPicks';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import CancelIcon from '@mui/icons-material/Cancel';
import { userId } from '../../../lib/user';
import { useStore } from '../../../stores/useStore.ts';

const fieldLabels: Record<keyof SuperLottoUserPick, string> = {
  id: 'Id',
  userId: 'User Id',
  firstPick: '1st Number',
  secondPick: '2nd Number',
  thirdPick: '3rd Number',
  fourthPick: '4th Number',
  fifthPick: '5th Number',
  megaPick: 'Mega Number',
};

const schema = yup.object({
  id: yup.string().required(),
  userId: yup.string().required(),
  firstPick: yup.number().typeError('Must be 1-47').required('Must be 1-47').min(1, 'Must be 1-47').max(47, 'Must be 1-47'),
  secondPick: yup.number().typeError('Must be 1-47').required('Must be 1-47').min(1, 'Must be 1-47').max(47, 'Must be 1-47'),
  thirdPick: yup.number().typeError('Must be 1-47').required('Must be 1-47').min(1, 'Must be 1-47').max(47, 'Must be 1-47'),
  fourthPick: yup.number().typeError('Must be 1-47').required('Must be 1-47').min(1, 'Must be 1-47').max(47, 'Must be 1-47'),
  fifthPick: yup.number().typeError('Must be 1-47').required('Must be 1-47').min(1, 'Must be 1-47').max(47, 'Must be 1-47'),
  megaPick: yup.number().typeError('Must be 1-27').required('Must be 1-27').min(1, 'Must be 1-27').max(27, 'Must be 1-27'),
});

export default function UserPickEntryGrid() {


  interface ConfirmDialogProps {
    open: boolean;
    content: string;
    onConfirm: () => void;
    onCancel: () => void;
    title?: string;
    disableEscapeKeyDown?: boolean;
  }

  const ConfirmDialog = ({
    open,
    content,
    onConfirm,
    onCancel,
    title = 'Confirm',
    disableEscapeKeyDown = false,
  }: ConfirmDialogProps) => {
    return (
      <Dialog
        open={open}
        onClose={(_, reason) => {
          if (reason === 'escapeKeyDown' && disableEscapeKeyDown) return;
          onCancel();
        }}
        disableEscapeKeyDown={disableEscapeKeyDown}
      >
        <DialogTitle>{title}</DialogTitle>
        <DialogContent>{content}</DialogContent>
        <DialogActions>
          <Button onClick={onCancel}>Cancel</Button>
          <Button onClick={onConfirm} color="primary" variant="contained">
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    );
  };


  const [open, setOpen] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedRow, setSelectedRow] = useState<SuperLottoUserPick | null>(null);
  const firstInputRef = useRef<HTMLInputElement | null>(null);
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);
  const [confirmOpen, setConfirmOpen] = useState(false);
  const { uiStore } = useStore();
  const queryClient = useQueryClient();
  const [editingPick, setEditingPick] = useState<SuperLottoUserPick | null>(null);
  const [isEditActive, setIsEditActive] = useState("");

  const isCompact = useMediaQuery('(max-width:768px)');

  const { control, handleSubmit, reset, setError, formState: { errors } } = useForm<SuperLottoUserPick>({
    resolver: yupResolver(schema),
  });

  const { data, isLoading, createPickForUser, deletePickForUser, updatePickForUser } = useLottoPicks();

  useEffect(() => {
    if (open && !isCompact) {
      setTimeout(() => {
        firstInputRef.current?.focus();
        firstInputRef.current?.select();
      }, 0);
    }
  }, [open, isCompact]);

  const handleEdit = (pick: SuperLottoUserPick) => {
    setIsEditActive(pick.id);
    setEditingPick(pick);
    reset(pick);
    setOpen(true);
    setDrawerOpen(false);
  };

  const confirmDelete = () => {
    if (!pendingDeleteId) return;
    toast.promise(
      new Promise((resolve, reject) => {
        deletePickForUser.mutate(pendingDeleteId, {
          onSuccess: () => {
            resolve(true);
            queryClient.invalidateQueries({ queryKey: ['winningPicksForUser'] });
            uiStore.isIdle();
            setConfirmOpen(false);
            setDrawerOpen(false);
          },
          onError: () => {
            reject();
            uiStore.isIdle();
          },
        });
      }),
      {
        loading: 'Deleting pick...',
        success: 'Pick deleted',
        error: 'Delete failed!',
      }
    );
  };

  const handleAdd = () => {
    setEditingPick(null);
    reset({ id: '00000000-0000-0000-0000-000000000000', userId: `${userId}` });
    setOpen(true);
  };

  const onSubmit = (data: SuperLottoUserPick) => {
    uiStore.isBusy();
    const mutationFn = editingPick ? updatePickForUser : createPickForUser;
    toast.promise(
      new Promise<void>((resolve, reject) => {
        mutationFn.mutate(data, {
          onSuccess: (newPickId) => {
            if (mutationFn == createPickForUser) {
              setIsEditActive(newPickId);
              setPaginationModel((prev) => ({ ...prev, page: 0 })); // go to first page
            }
            queryClient.invalidateQueries({ queryKey: ['winningPicksForUser'] });
            uiStore.isIdle();
            setOpen(false);
            resolve();
            console.log("scrolling...");
            window.scrollTo({ top: 0, behavior: 'smooth' });
          },
          onError: (error: any) => {
            const apiErrorMessage = error?.response?.data?.error || error?.message || 'An unknown error occurred';
            if (apiErrorMessage.includes('CK_LottoChecker_SuperLottoUserPick')) {
              setError('root.serverError', { type: 'manual', message: 'Duplicate pick numbers are not allowed!' });
            } else if (apiErrorMessage.includes('UC_LottoChecker_SuperLottoUserPicks_RowCheckSum')) {
              setError('root.serverError', {
                type: 'manual',
                message: editingPick ? 'Pick already exists, not updated!' : 'Pick not created (pick already exists)!'
              });
            } else {
              toast.error('Something went wrong, please try again.');
            }
            uiStore.isIdle();
            reject(error);
          },
        });
      }),
      {
        loading: editingPick ? 'Updating pick...' : 'Creating pick...',
        success: editingPick ? 'Pick updated' : 'Pick created',
        error: '',
      }
    );
  };

  const requestDelete = (id: string) => {
    setIsEditActive(id)
    setPendingDeleteId(id);
    setConfirmOpen(true);
  };

  const columns: GridColDef[] = [
    { field: 'firstPick', sortable: false, flex: 1, minWidth: isCompact ? 22 : 80, headerName: '1st', align: 'center', headerAlign: 'center' },
    { field: 'secondPick', sortable: false, flex: 1, minWidth: isCompact ? 22 : 80, headerName: '2nd', align: 'center', headerAlign: 'center' },
    { field: 'thirdPick', sortable: false, flex: 1, minWidth: isCompact ? 22 : 80, headerName: '3rd', align: 'center', headerAlign: 'center' },
    { field: 'fourthPick', sortable: false, flex: 1, minWidth: isCompact ? 22 : 80, headerName: '4th', align: 'center', headerAlign: 'center' },
    { field: 'fifthPick', sortable: false, flex: 1, minWidth: isCompact ? 22 : 80, headerName: '5th', align: 'center', headerAlign: 'center' },
    { field: 'megaPick', sortable: false, flex: 1, minWidth: isCompact ? 50 : 90, headerName: 'Mega', headerClassName: 'mega-column-header', align: 'center', headerAlign: 'center' },
    ...(!isCompact ? [{
      field: ' ',
      flex: 1,
      minWidth: 90,
      renderCell: (params: GridRenderCellParams<SuperLottoUserPick>) => (
        <Box display="flex" gap={1} justifyContent="center" alignItems="flex-end" width="100%" >
          <Tooltip title="Edit this pick">
            <Button
              onClick={() => {
                handleEdit(params.row);
              }}
              sx={{ mt: 1 }}
            >
              <EditIcon fontSize="small" />
            </Button>
          </Tooltip>

          <Tooltip title="Delete this pick">
            <Button
              onClick={() => {
                requestDelete(params.row.id);
              }}
            >
              <DeleteIcon fontSize="small" />
            </Button>
          </Tooltip>
        </Box>
      )
    }] : [])
  ];

  const [paginationModel, setPaginationModel] = useState({ page: 0, pageSize: 3 });

  function CustomFooter() {
    return (
      <GridFooterContainer sx={{ justifyContent: 'space-between', px: 2 }}>
        <Button
          variant="contained"
          onClick={handleAdd}
          size={isCompact ? 'small' : 'medium'}
          sx={isCompact ? { fontSize: '0.65rem', px: 1, py: 0.5 } : {}}
        >
          Add Pick
        </Button>
        <GridPagination />
      </GridFooterContainer>
    );
  }

  return (
    <Box>
      <Card elevation={3} sx={{ borderRadius: 3, width: '100%', flexGrow: 1 }}>
        <Box sx={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
          <Typography
            variant={isCompact ? 'h6' : 'h5'}
            sx={{ textAlign: 'center', color: 'primary.main', my: 1 }}
          >
            Your Super Lotto Tickets
          </Typography>
          {isCompact && Array.isArray(data) && data.length > 0 && (
            <Typography
              variant="caption"
              display="block"
              textAlign="center"
              mb={1}
              fontWeight="bold"
            >
              Tap a row to edit or delete a pick
            </Typography>
          )}
          <DataGrid

            autoHeight
            sx={{
              // Set font size responsive to screen size
              fontSize: isCompact? { xs: '0.75rem', sm: '0.9rem' } : '1.2rem' ,

              // Disable hover interaction for native title tooltips
              '& .MuiDataGrid-cell': {
                '&[title]': {
                  pointerEvents: 'none',
                },
              },

              // Style column headers: reduce line height and adjust font size based on compact mode
              '& .MuiDataGrid-columnHeaders': {
                lineHeight: 1.1,
                fontSize: isCompact ? '0.65rem' : '1.1rem',
              },

              // Remove outer border from the grid
              border: 'none',

              // Prevent default focus outline on cells for cleaner look
              '& .MuiDataGrid-cell:focus, & .MuiDataGrid-cell:focus-within': {
                outline: 'none',
              },

              // Also remove outline when header cells get focus
              '& .MuiDataGrid-columnHeader:focus, & .MuiDataGrid-columnHeader:focus-within': {
                outline: 'none',
              },
            }}


            getRowClassName={(params) => {
              if (isEditActive === params.row.id)
                return 'editing-row';
              else
                return 'not-editing-row';
            }}

            slots={{ footer: CustomFooter }}
            disableColumnMenu
            disableColumnResize
            rows={data ?? []}
            paginationModel={paginationModel}
            onRowClick={(params) => {
              if (isCompact) {
                setSelectedRow(params.row);
                setIsEditActive(params.row.id);
                setDrawerOpen(true);
              } else {
              }
            }}
            pageSizeOptions={[3]}
            onPaginationModelChange={setPaginationModel}
            localeText={{
              noRowsLabel: 'No tickets entered yet. Add your pick numbers from your SUPER LOTTO ticket(s) to see if your picks won a prize!',
            }}
            columnVisibilityModel={{ id: false, userId: false }}
            columns={columns}
            loading={isLoading}
            getRowId={(row) => row.id}
          />
        </Box>
      </Card>


      <ConfirmDialog
        open={confirmOpen}
        content="100% sure you want to delete this pick?"
        onConfirm={confirmDelete}
        onCancel={() => {
          setConfirmOpen(false);
          setDrawerOpen(false);
        }}
        disableEscapeKeyDown
      />

      <Dialog open={open} onClose={() => { setOpen(false); }} fullWidth maxWidth="sm" disableEscapeKeyDown>
        <form onSubmit={handleSubmit(onSubmit)} style={{ display: 'contents' }}>
          <DialogTitle sx={{ fontSize: '1rem', py: 1 }}>
            {editingPick ? 'Edit Super Lotto Pick' : 'Add a Super Lotto Pick'}
          </DialogTitle>
          <DialogContent>
            {errors.root?.serverError && (
              <Typography color="error" sx={{ mb: 2 }}>
                {errors.root.serverError.message}
              </Typography>
            )}
            <Box display="grid" gap={1} mt={1}>
              {Object.entries(fieldLabels)
                .filter(([fieldName]) => fieldName !== 'id' && fieldName !== 'userId')
                .map(([fieldName, label], i) => (
                  <Controller
                    key={fieldName}
                    name={fieldName as keyof SuperLottoUserPick}
                    control={control}
                    render={({ field, fieldState }) => (
                      <TextField
                        {...field}
                        type="text"
                        inputMode="numeric"
                        onKeyDown={(e) => {
                          if (
                            !/^\d$/.test(e.key) &&
                            e.key !== 'Backspace' &&
                            e.key !== 'Delete' &&
                            e.key !== 'Tab' &&
                            e.key !== 'ArrowLeft' &&
                            e.key !== 'ArrowRight'
                          ) {
                            e.preventDefault();
                          }
                        }}
                        onPaste={(e) => {
                          const pasted = e.clipboardData.getData('Text');
                          if (!/^\d+$/.test(pasted)) {
                            e.preventDefault();
                          }
                        }}
                        onBlur={(e) => {
                          const raw = e.target.value;
                          const stripped = raw.replace(/^0+(?!$)/, '');
                          field.onChange(stripped ? Number(stripped) : '');
                        }}
                        value={
                          typeof field.value === 'number'
                            ? field.value.toString()
                            : field.value?.replace(/^0+(?!$)/, '')
                        }
                        onChange={(e) => {
                          const input = e.target.value;
                          if (/^\d*$/.test(input)) {
                            field.onChange(input);
                          }
                        }}
                        label={label}
                        inputRef={!isCompact && i === 0 ? firstInputRef : undefined}
                        error={!!fieldState.error}
                        helperText={fieldState.error?.message}
                        fullWidth
                        size={isCompact ? 'small' : 'medium'}
                        margin={isCompact ? 'dense' : 'normal'}
                        inputProps={{
                          inputMode: 'numeric',
                          pattern: '[0-9]*',
                        }}
                      />

                    )}
                  />
                ))}
            </Box>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => { setOpen(false); }} size={isCompact ? 'small' : 'medium'}>
              Cancel
            </Button>
            <Button type="submit" variant="contained" size={isCompact ? 'small' : 'medium'}>
              {editingPick ? 'Update' : 'Create'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      <SwipeableDrawer
        anchor="bottom"
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        onOpen={() => { }}
      >
        <Box>
          <List>
            {/* Edit Button */}
            <ListItemButton onClick={() => handleEdit(selectedRow!)}>
              <ListItemIcon><EditIcon /></ListItemIcon>
              <ListItemText primary="Edit Pick" />
            </ListItemButton>

            <Divider />

            {/* Delete Button */}
            <ListItemButton onClick={() => requestDelete(selectedRow!.id)}>
              <ListItemIcon><DeleteIcon /></ListItemIcon>
              <ListItemText primary="Delete Pick" />
            </ListItemButton>

            <Divider />

            {/* Cancel Button with Icon */}
            <ListItemButton onClick={() => setDrawerOpen(false)}>
              <ListItemIcon><CancelIcon /></ListItemIcon>
              <ListItemText primary="Cancel" />
            </ListItemButton>
          </List>
        </Box>
      </SwipeableDrawer>
    </Box>

  );
}