// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include <mkl.h>
#include <mkl_df_defines.h>

#include <iostream>
#include <vector>
#include <cmath>
#include <memory>

enum class ErrorEnum { NO, INIT, CHECK, SOLVE, JACOBI, GET, MYDELETE, RCI };

struct Help_info
{
    int non_uniform_grid_len;
    double* non_uniform_grid;
    double* values_non_uniform_grid;
    int m;
    double* borders;
    double* scoeff;
    MKL_INT s_order;
    MKL_INT s_type;
    MKL_INT bc_type;
};

void compute_spline(DFTaskPtr task, Help_info& info, double*& result) {
    int status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
    if (status != DF_STATUS_OK) throw "Couldn't construct spline";

    // Подсчет значений сплайна на неравномерной сетки для дальнейшей оптимизации
    int nDorder = 1;
    MKL_INT dorder[] = { 1 };
    int nnon_zero_ders = 1;
    result = new double[info.non_uniform_grid_len * nnon_zero_ders];
    status = dfdInterpolate1D(task,
        DF_INTERP, DF_METHOD_PP,
        info.non_uniform_grid_len, info.non_uniform_grid,
        DF_NON_UNIFORM_PARTITION,
        nDorder, dorder,
        NULL,
        result, DF_NO_HINT, NULL);
    if (status != DF_STATUS_OK) throw "Couldn't interpolate";
}

void Function(MKL_INT* nX, MKL_INT* m, double* values_on_uniform_grid, double* f, void* user_data)
{
    Help_info info = *((Help_info*)user_data);
    int status = -1;
    DFTaskPtr task;
    double* spline_with_derivatives = nullptr;

    try {
        int m_inside = info.m;
        double* borders = info.borders;
        MKL_INT s_order = info.s_order;
        MKL_INT s_type = info.s_type;
        MKL_INT bc_type = info.bc_type;
        double* scoeff = info.scoeff;

        status = dfdNewTask1D(&task,
            m_inside, borders,
            DF_UNIFORM_PARTITION,
            1, values_on_uniform_grid,
            DF_NO_HINT);
        if (status != DF_STATUS_OK) throw "Couldn't create task";

        status = dfdEditPPSpline1D(task,
            s_order, s_type,
            bc_type, NULL,
            DF_NO_IC, NULL,
            scoeff, DF_NO_HINT);
        if (status != DF_STATUS_OK) throw "Couldn't configure task";

        compute_spline(task, info, spline_with_derivatives);

        // x  хранит значения на неравномерной сетке
        for (int i = 0; i < info.non_uniform_grid_len; ++i)
            f[i] = std::pow((spline_with_derivatives[i] - info.values_non_uniform_grid[i]), 2);

        delete[] spline_with_derivatives;
        spline_with_derivatives = nullptr;

        status = dfDeleteTask(&task);
        if (status != DF_STATUS_OK) std::cerr << "Error deleting task" << std::endl;
    }
    catch (const char* errMsg) {
        std::cerr << errMsg << std::endl;
        if (spline_with_derivatives != nullptr) delete[] spline_with_derivatives;
        if (task != nullptr) dfDeleteTask(&task);
    }
}

void calculate_spline(MKL_INT* nX, double* storage, MKL_INT* m, double* values_on_uniform_grid, void* user_data)
{
    Help_info info = *((Help_info*)user_data);
    int status = -1;
    DFTaskPtr task;
    double* spline_with_derivatives = nullptr;

    try {
        int m_inside = info.m;
        double* borders = info.borders;
        MKL_INT s_order = info.s_order;
        MKL_INT s_type = info.s_type;
        MKL_INT bc_type = info.bc_type;
        double* scoeff = info.scoeff;

        status = dfdNewTask1D(&task,
            m_inside, borders,
            DF_UNIFORM_PARTITION,
            1, values_on_uniform_grid,
            DF_NO_HINT);
        if (status != DF_STATUS_OK) throw "Couldn't create task";

        status = dfdEditPPSpline1D(task,
            s_order, s_type,
            bc_type, NULL,
            DF_NO_IC, NULL,
            scoeff, DF_NO_HINT);
        if (status != DF_STATUS_OK) throw "Couldn't configure task";

        compute_spline(task, info, spline_with_derivatives);

        // x  хранит значения на неравномерной сетке
        for (int i = 0; i < info.non_uniform_grid_len; ++i)
            storage[i] = spline_with_derivatives[i];

        delete[] spline_with_derivatives;
        spline_with_derivatives = nullptr;

        status = dfDeleteTask(&task);
        if (status != DF_STATUS_OK) std::cerr << "Error deleting task" << std::endl;
    }
    catch (const char* errMsg) {
        std::cerr << errMsg << std::endl;
        if (spline_with_derivatives != nullptr) delete[] spline_with_derivatives;
        if (task != nullptr) dfDeleteTask(&task);
    }
}

extern "C" _declspec(dllexport)
void DllSpline(
    int nX,
    double* X,
    int nY,
    double* Y,
    int m,
    double* values_on_uniform_grid,
    double* splineValues,
    int* stop_reason,
    int max_iterations,
    int* actual_number_of_iterations,
    double* addGrid,
    double* addSplineData,
    int addSize)
{
    MKL_INT s_order = DF_PP_CUBIC;
    MKL_INT s_type = DF_PP_NATURAL;
    MKL_INT bc_type = DF_BC_FREE_END;

    // Using smart pointers for memory management
    std::unique_ptr<double[]> scoeff(new double[1 * (m - 1) * s_order]);
    std::unique_ptr<double[]> fvec(new double[nX]);
    std::unique_ptr<double[]> fjac(new double[nX * m]);

    MKL_INT niter1 = max_iterations;
    MKL_INT niter2 = 100;
    MKL_INT ndone_iter = 0;
    double rs = 10;

    const double eps[] = { 1.0E-12, 1.0E-12, 1.0E-12, 1.0E-12, 1.0E-12, 1.0E-12 };
    double jac_eps = 1.0E-8;

    double res_initial = 0;
    double res_final = 0;
    MKL_INT stop_criteria;
    MKL_INT check_data_info[4];
    ErrorEnum error = ErrorEnum::NO;

    _TRNSP_HANDLE_t handle = NULL;

    try
    {
        double borders[2]{ X[0], X[nX - 1] };

        Help_info help;
        help.non_uniform_grid_len = nX;
        help.non_uniform_grid = X;
        help.values_non_uniform_grid = Y;
        help.m = m;
        help.borders = borders;
        help.scoeff = scoeff.get();
        help.s_order = s_order;
        help.s_type = s_type;
        help.bc_type = bc_type;

        MKL_INT ret = dtrnlsp_init(&handle, &m, &nX, values_on_uniform_grid, eps, &niter1, &niter2, &rs);
        if (ret != TR_SUCCESS) throw ErrorEnum::INIT;

        ret = dtrnlsp_check(&handle, &m, &nX, fjac.get(), fvec.get(), eps, check_data_info);
        if (ret != TR_SUCCESS) throw ErrorEnum::CHECK;

        MKL_INT RCI_Request = 0;

        // Iterative process
        bool skip_spline_construct = false;
        while (true)
        {
            if (!skip_spline_construct) {
                skip_spline_construct = true;
            }
            ret = dtrnlsp_solve(&handle, fvec.get(), fjac.get(), &RCI_Request);
            if (ret != TR_SUCCESS) throw ErrorEnum::SOLVE;
            if (RCI_Request == 0) continue;
            else if (RCI_Request == 1) Function(&nX, &m, values_on_uniform_grid, fvec.get(), &help);
            else if (RCI_Request == 2)
            {
                ret = djacobix(Function, &m, &nX, fjac.get(), values_on_uniform_grid, &jac_eps, &help);
                if (ret != TR_SUCCESS) throw ErrorEnum::JACOBI;
            }
            else if (RCI_Request >= -6 && RCI_Request <= -1) break;
            else throw ErrorEnum::RCI;
        }

        // Completion of the iterative process
        ret = dtrnlsp_get(&handle, &ndone_iter, &stop_criteria, &res_initial, &res_final);
        if (ret != TR_SUCCESS) throw ErrorEnum::GET;

        ret = dtrnlsp_delete(&handle);
        if (ret != TR_SUCCESS) throw ErrorEnum::MYDELETE;

        // Save results
        std::unique_ptr<double[]> storage(new double[nX]);
        calculate_spline(&nX, storage.get(), &m, values_on_uniform_grid, &help);
        for (int i = 0; i < nX; ++i)
        {
            splineValues[i] = storage[i];
        }

        *stop_reason = stop_criteria;
        *actual_number_of_iterations = ndone_iter;
        help.non_uniform_grid = addGrid;
        help.non_uniform_grid_len = addSize;
        calculate_spline(&addSize, addSplineData, &m, values_on_uniform_grid, &help);
    }
    catch (ErrorEnum _error) { error = _error; }
    catch (const char* str)
    {
        std::cerr << std::string(str) << std::endl;
        std::cout << std::string(str) << std::endl;
    }
}