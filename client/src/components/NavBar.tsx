import { useState } from "react";
import {
  Box,
  AppBar,
  Toolbar,
  Typography,
  Container,
  MenuItem,
  IconButton,
  Menu,
  useMediaQuery,
  useTheme
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import { NavLink } from "react-router-dom";
import MenuItemLink from "../components/MenuItemLink";
import logo from '/lotto-checker.webp';

export default function NavBar() {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));

  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };
  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="static">
        <Container maxWidth="xl">
          <Toolbar sx={{ justifyContent: "space-between", flexWrap: "wrap" }}>
            <MenuItem component={NavLink} to="/super-lotto" sx={{ display: "flex", flexDirection: "column", alignItems: "flex-start", textDecoration: "none" }}>
              <Box sx={{ display: "flex", alignItems: "center" }}>
                <img src={logo} alt="Site Logo" style={{ width: 36, height: 36, marginRight: 8 }} />
                <Box>
                  <Typography variant="h5" fontWeight="bold" noWrap>
                    Lotto Checker
                  </Typography>
                  <Typography sx={{ fontSize: '0.85em', fontWeight: 'normal', marginTop: '-0.25rem' }}>
                    by Gary Sloan
                  </Typography>
                </Box>
              </Box>
            </MenuItem>
            {isMobile ? (
              <>
                <IconButton
                  edge="end"
                  color="inherit"
                  aria-label="menu"
                  onClick={handleMenuOpen}
                >
                  <MenuIcon />
                </IconButton>
                <Menu
                  anchorEl={anchorEl}
                  open={Boolean(anchorEl)}
                  onClose={handleMenuClose}
                  anchorOrigin={{
                    vertical: "bottom",
                    horizontal: "right",
                  }}
                  transformOrigin={{
                    vertical: "top",
                    horizontal: "right",
                  }}
                >
                  <MenuItem onClick={handleMenuClose} component={NavLink} to="/super-lotto">
                    Super Lotto
                  </MenuItem>

                  <MenuItem onClick={handleMenuClose} component={NavLink} to="/powerBI-Report">
                    Mega Ball Occurrence
                  </MenuItem>

                  <MenuItem onClick={handleMenuClose} component={NavLink} to="/tableau-Report">
                    Draw Ball Occurrence
                  </MenuItem>

                  <MenuItem onClick={handleMenuClose} component={NavLink} to="/about">
                    About
                  </MenuItem>
                </Menu>
              </>
            ) : (
              <Box sx={{ display: "flex", gap: 2 }}>
                <MenuItemLink to="/super-lotto">Super Lotto</MenuItemLink>
                <MenuItemLink to="/powerBI-Report">Mega Ball Occurrence (Power BI)</MenuItemLink>
                <MenuItemLink to="/tableau-Report">Draw Ball Occurrence (Tableau)</MenuItemLink>
                <MenuItemLink to="/about">About</MenuItemLink>
              </Box>
            )}
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}
