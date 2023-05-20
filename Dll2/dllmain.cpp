// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include "mkl.h"

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" _declspec(dllexport)
void CalculateMKL(int nodes_count_old, double* grid_uniform, double* grid_nonuniform, double* funcion_values_old, bool grid_type, int unodes_num,
    double* derivatives_bounds, double* left_bound, double* right_bound, double* calculated_integrals, double* calculated_values, int& status)
{
    DFTaskPtr task;
    status = DF_STATUS_OK;
    if (grid_type)
    {
        // http://www.physics.ntua.gr/~konstant/HetCluster/intel12.1/mkl/mkl_manual/GUID-E308EE83-B72B-4570-B4B3-AD6EAE08DC0A.htm
        status = dfdNewTask1D(&task, nodes_count_old, grid_uniform, DF_UNIFORM_PARTITION, 1, funcion_values_old, DF_MATRIX_STORAGE_ROWS);
        if (status != DF_STATUS_OK)
        {
            return;
        }
    }
    else
    {
        // http://www.physics.ntua.gr/~konstant/HetCluster/intel12.1/mkl/mkl_manual/GUID-E308EE83-B72B-4570-B4B3-AD6EAE08DC0A.htm
        status = dfdNewTask1D(&task, nodes_count_old, grid_nonuniform, DF_NON_UNIFORM_PARTITION, 1, funcion_values_old, DF_MATRIX_STORAGE_ROWS);
        if (status != DF_STATUS_OK)
        {
            return;
        }
    }

    // 
    double* scoeff = new double[1 * 4 * (nodes_count_old - 1)];
    //double* scoeff = new double[2 * DF_PP_CUBIC * (nodes_count_old - 1)];
    status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER, derivatives_bounds, DF_NO_IC, NULL, scoeff, DF_NO_HINT);
    if (status != DF_STATUS_OK)
    {
        return;
    }

    //http://www.physics.ntua.gr/~konstant/HetCluster/intel12.1/mkl/mkl_manual/GUID-C52F3D6B-A29F-4129-B985-2A58EED4EADA.htm
    status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
    if (status != DF_STATUS_OK)
    {
        return;
    }

    // http://www.physics.ntua.gr/~konstant/HetCluster/intel12.1/mkl/mkl_manual/GUID-5B42A1E3-F0B3-43DC-9AEB-19F173605A19.htm
    int dorder[3] = { 1, 1, 1 };
    status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, unodes_num, grid_uniform, DF_UNIFORM_PARTITION, 3, dorder, NULL, calculated_values, DF_MATRIX_STORAGE_ROWS, NULL);
    if (status != DF_STATUS_OK)
    {
        return;
    }

    // http://www.physics.ntua.gr/~konstant/HetCluster/intel12.1/mkl/mkl_manual/GUID-382F7F95-3E40-4F48-8930-D2FACD09C2B2.htm
    status = dfdIntegrate1D(task, DF_METHOD_PP, 1, left_bound, DF_NON_UNIFORM_PARTITION, right_bound, DF_NON_UNIFORM_PARTITION, NULL, NULL, calculated_integrals, DF_NO_HINT);
    if (status != DF_STATUS_OK)
    {
        return;
    }

    // http://www.physics.ntua.gr/~konstant/HetCluster/intel12.1/mkl/mkl_manual/index.htm#GUID-382F7F95-3E40-4F48-8930-D2FACD09C2B2.htm
    status = dfDeleteTask(&task);
}
