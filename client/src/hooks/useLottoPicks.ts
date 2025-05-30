import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { userId } from '../lib/user';
export const useLottoPicks = (id?: string) => {
    const queryClient = useQueryClient();

    const { data: winningPicksForUserData, isPending: isLoadingWinningPicksForUser } = useQuery({
        queryKey: ['winningPicksForUser', id],
        queryFn: async () => {
            const response = await agent.get<WinningPicksForUser[]>(`/LottoChecker/GetWinningSuperLottoDrawsForUser?userId=${userId}`);
            return response.data;
        },
    });

    const { data, isPending: isLoading } = useQuery({
        queryKey: ['superLottoPicksForUser', id],
        queryFn: async () => {

            const response = await agent.get<SuperLottoUserPick[]>(`/LottoChecker/GetSuperLottoPicksForUser?userId=${userId}`);
            return response.data;
        },
    });

    const createPickForUser = useMutation({
        mutationFn: async (superLottoUserPick: SuperLottoUserPick) => {
            const response = await agent.post('LottoChecker/CreateSuperLottoPickForUser', superLottoUserPick);
            return response.data;
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['superLottoPicksForUser']
            })
        }
    });

    const updatePickForUser = useMutation({
        mutationFn: async (superLottoUserPick: SuperLottoUserPick) => {
            const response = await agent.put('LottoChecker/UpdateSuperLottoPickForUser', superLottoUserPick);
            return response.data;
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['superLottoPicksForUser']
            })
        }
    });

    const deletePickForUser = useMutation({
        mutationFn: async (pickId: string) => {
            const response = await agent.delete(`LottoChecker/DeleteSuperLottoPickForUser?PickId=${pickId}`);
            return response.data;
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['superLottoPicksForUser']
            })
        }
    });

    return {
        data,
        isLoading,
        createPickForUser,
        deletePickForUser,
        updatePickForUser,
        isLoadingWinningPicksForUser,
        winningPicksForUserData
    }

}