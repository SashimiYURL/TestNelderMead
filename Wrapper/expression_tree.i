%module expression_tree

// Заголовочные файлы C++ (указываем относительные пути)
%{
#include "./include/dll_api.h"
#include "./include/internal_func.h"
#include "./include/node_classes.h"
#include "./include/expression_tree.h"
%}

// Упрощаем макрос экспорта (если он мешает)
#define NELDERMID_API

// Включаем поддержку std::string и std::vector
%include "std_string.i"
%include "std_vector.i"

// Преобразуем std::vector<double> в C# List<double>
namespace std {
    %template(DoubleVector) vector<double>;
}

// Игнорируем приватные методы/конструкторы (если не нужны)
%ignore ExpressionTree::ExpressionTree(TreeNode*, int);

class ExpressionTree {
public:
    ~ExpressionTree();
    static ExpressionTree* create_tree(const std::string& function_str);
    double evaluate(const std::vector<double>& variables);
    bool check_number_variables(int number_variables);
    std::string json_tree();
    int get_number_variables();
};

// Включаем только публичный API
#include "../NelderMead_dll/dll/include/expression_tree.h"
