%module NelderMid

%nodefaultctor ExpressionTree;
%ignore ExpressionTree::ExpressionTree(TreeNode* const, int const);

%{
#include "./include/dll_api.h"
#include "./include/internal_func.h"
#include "./include/node_classes.h"
#include "./include/expression_tree.h"
%}

#define NELDERMID_API

%include "std_string.i"
%include "std_vector.i"

namespace std {
    %template(DoubleVector) vector<double>;
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

#include "../NelderMead_dll/dll/include/expression_tree.h"
