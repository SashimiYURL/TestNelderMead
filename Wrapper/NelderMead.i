%module NelderMead

%nodefaultctor ExpressionTree;
%ignore ExpressionTree::ExpressionTree(TreeNode* const, int const);

%{
#include "./include/dll_api.h"
#include "./include/internal_func.h"
#include "./include/node_classes.h"
#include "./include/expression_tree.h"
#include "./include/nelder_mead_method.h"
%}

#define NELDERMID_API

%include "std_string.i"
%include "std_vector.i"

namespace std {
    %template(DoubleVector) vector<double>;
    %template(DoubleVectorVector) vector<vector<double>>;
    %template(DoubleVectorVectorVector) vector<vector<vector<double>>>;
}

%include "std_except.i"

%exception {
    try {
        $action
    } catch (std::logic_error e) {
        SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, e.what());
    } catch (std::logic_error* e) {
        SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, e->what());
    } catch(...) {
        SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, "Unknown exception");
    }
}

class ExpressionTree {
public:
    ~ExpressionTree();
    static ExpressionTree* create_tree(const std::string& function_str);
    double evaluate(const std::vector<double>& variables);
    bool check_number_variables(int number_variables);
    std::string json_tree();
    int get_number_variables();
};

class NelderMeadMethod {
public:
    NelderMeadMethod(ExpressionTree* expression_tree_, double reflection_ = 1,
                   double expansion_ = 2, double contraction_ = 0.5,
                   double homothety_ = 0.5, double dispersion_ = 0.0001);
    
    void generate_simplex(double step, const std::vector<double>& x0 = {});
    void set_simplex(const std::vector<std::vector<double>>& simplex_);
    std::vector<std::vector<std::vector<double>>> minimum_search(int number_steps = 10000);
};

#include "./include/expression_tree.h"
#include "./include/nelder_mead_method.h"
