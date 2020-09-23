namespace Assets.Scripts.Levels.Generation.RoomBuilder
{
    public enum RoomTypeEnum
    {
        //1-1 (A)
        A_Open_1_1,

        A_EndRoom_1_1,
        A_Connector_1_1,
        A_Elbow_1_1,
        A_BackWall_1_1,

        //1-2 (B)
        B_BackWall_1_2,

        B_BackWall_Long_1_2,
        B_Connector_1_2,
        B_Connector_Z_Back_1_2,
        B_Connector_Z_Forward_1_2,
        B_EndRoom_1_2,
        B_EndRoom_Small_1_2,
        B_Open_1_2,

        //2-2-E (C)
        C_Connector_A_2_2,

        C_Connector_D_2_2,
        C_EndRoom_A_2_2,
        C_EndRoom_D_2_2,
        C_EndRoom_Small_A_2_2,
        C_EndRoom_Small_D_2_2,
        C_EndRoom_Small_Double_2_2,
        C_Open_2_2,
        C_Open_Corner_2_2,

        //2-2 (D)
        D_BackWall_2_2,

        D_Corner_2_2,
        D_Connector_2_2,
        D_EndRoom_2_2,
        D_Open_2_2,

        //2-3 (E)
        E_BackWall_2_3,

        E_BackWall_Long_2_3,
        E_BackWall_Long_Corners_2_3,
        E_Connector_2_3,
        E_Connector_Long_2_3,
        E_EndRoom_2_3,
        E_EndRoom_Long_2_3,
        E_Open_2_3,

        //3-3 (F)
        F_BackWall_3_3,

        F_Connector_3_3,
        F_Corner_3_3,
        F_Corners_All_3_3,
        F_Cross_3_3,
        F_EndRoom_3_3,
        F_Open_3_3,

        //3-4 (G)
        G_BackWall_3_4,

        G_BackWall_Long_3_4,
        G_Connector_3_4,
        G_Connector_Long_3_4,
        G_EndRoom_3_4,
        G_EndRoom_Long_3_4,
        G_Middle_3_4,
        G_Open_3_4,

        //4-4 (H)
        H_BackWall_4_4,

        H_Connector_4_4,
        H_Corner_4_4,
        H_Corners_All_4_4,
        H_Cross_4_4,
        H_EndRoom_4_4,
        H_Open_4_4,

        //T-Shape (I)
        I_Open_T,

        I_BackWall_T,
        I_EndRoom_T,
        I_EndRoom_Corner_B_T,
        I_EndRoom_Corner_F_T,
        I_EndRoom_Sides_T,

        //Cross (J)
        J_Open_X,

        J_EndRoom_X,
        J_EndRoom_Across_X,
        J_EndRoom_Corner_B_X,
        J_EndRoom_Corner_F_X,
        J_EndRoom_Triple_X,
    }
}