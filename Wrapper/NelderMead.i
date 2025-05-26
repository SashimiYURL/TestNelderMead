%module NelderMead

%{
#include "./include/dll_api.h"
#include "./include/ifunction.h"
#include "./include/point.h"
#include "./include/simplex.h"
#include "./include/simplex_history.h"
#include "./include/expression_tree.h"
#include "./include/nelder_mead_method.h"
%}

#define NELDERMID_API

%include "std_string.i"
%include "std_vector.i"
%include "std_except.i"

// Указываем SWIG не создавать конструкторы по умолчанию
%nodefaultctor Point;
%nodefaultctor Simplex;
%nodefaultctor ExpressionTree;

// Объявляем шаблоны для работы с векторами
namespace std {
    %template(DoubleVector) vector<double>;
    %template(DoubleVectorVector) vector<vector<double>>;
    %template(DoubleVectorVectorVector) vector<vector<vector<double>>>;
    %template(PointVector) vector<Point*>;
}

// Обработка исключений
%exception {
    try {
        $action
    } catch (std::exception& e) {
        SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, e.what());
        return $null;
    } catch (...) {
        SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, "Unknown exception");
        return $null;
    }
}

// Интерфейс IFunction
class IFunction {
public:
    virtual ~IFunction();
    virtual double evaluate(const Point* variables) const = 0;
    virtual int get_number_variables() = 0;
    virtual bool check_number_variables(int number_variables) = 0;
};

// Класс Point - только фабричные методы
class Point {
public:
    static Point* create_point(const std::vector<double>& coords, size_t N);
    double get(size_t index) const;
    void set(double value, size_t index);
    size_t dimensions() const;
    Point* clone() const;
    std::vector<double> get_vector_point();
    ~Point();
};

// Класс Simplex - только фабричные методы
class Simplex {
public:
    static Simplex* create_simplex(const std::vector<Point*>& coords_list);
    static Simplex* create_simplex(double step, size_t dimension, const Point* x0 = nullptr);
    void sort_simplex(const IFunction* function);
    Point* centroid(int exclude_index);
    Point* get_vertex(size_t index) const;
    void set_vertex(Point* vertex, size_t index);
    size_t dimension() const;
    size_t vertex_count() const;
    Simplex* clone() const;
    ~Simplex();
};

// Класс SimplexHistory
class SimplexHistory {
public:
    void add_simplex(Simplex* simplex);
    std::vector<std::vector<std::vector<double>>> get_vector_history();
    ~SimplexHistory();
};

// Класс ExpressionTree
class ExpressionTree : public IFunction {
public:
    ~ExpressionTree();
    static ExpressionTree* create_tree(const std::string& function_str);
    double evaluate(const Point* variables) const override;
    bool check_number_variables(int number_variables) override;
    std::string json_tree();
    int get_number_variables() override;
};

// Класс NelderMeadMethod
class NelderMeadMethod {
public:
    NelderMeadMethod(IFunction* function_, double reflection_ = 1,
                   double expansion_ = 2, double contraction_ = 0.5,
                   double homothety_ = 0.5, double dispersion_ = 0.0001);
    
    void set_simplex(const Simplex* simplex_);
    SimplexHistory* minimum_search(int number_steps = 10000);
    ~NelderMeadMethod();
};

// Указание на то, что C# должен освобождать память для этих классов
%newobject ExpressionTree::create_tree;
%newobject Point::create_point;
%newobject Point::clone;
%newobject Simplex::create_simplex;
%newobject Simplex::centroid;
%newobject Simplex::clone;
%newobject NelderMeadMethod::minimum_search;