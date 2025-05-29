%module NelderMead

%{
#include "./include/dll_api.h"
#include "./include/ipoint.h"
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
%nodefaultctor IPoint;
%nodefaultctor Point;
%nodefaultctor Simplex;
%nodefaultctor ExpressionTree;
%nodefaultctor NelderMeadMethod;

// Объявляем шаблоны для работы с векторами
namespace std {
    %template(DoubleVector) vector<double>;
    %template(IPointVector) vector<IPoint*>;
    %template(SimplexVector) vector<Simplex*>;
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

// Интерфейс IPoint
class IPoint {
public:
    static IPoint* create_point(const std::vector<double>& coords, size_t N);
    virtual double get(size_t index) const = 0;
    virtual void set(double value, size_t index) = 0;
    virtual size_t dimensions() const = 0;
    virtual IPoint* clone() const = 0;
    virtual ~IPoint() = default;
};

// Класс Point
class Point : public IPoint {
public:
    explicit Point(const std::vector<double>& coords);
    double get(size_t index) const;
    void set(double value, size_t index);
    size_t dimensions() const;
    Point* clone() const;
    std::vector<double> get_vector_point();
    ~Point();
};

// Интерфейс IFunction
class IFunction {
public:
    virtual ~IFunction() = default;
    virtual double evaluate(const IPoint* variables) const = 0;
    virtual int get_number_variables() = 0;
};

// Класс Simplex
class Simplex {
public:
    static Simplex* create_simplex(const std::vector<IPoint*>& coords_list);
    static Simplex* create_simplex(double step, size_t dimension, const IPoint* x0 = nullptr);
    void sort_simplex(const IFunction* function);
    IPoint* centroid(int exclude_index);
    IPoint* get_vertex(size_t index) const;
    void set_vertex(IPoint* vertex, size_t index);
    size_t dimension() const;
    size_t vertex_count() const;
    Simplex* clone() const;
    ~Simplex();
};

// Класс SimplexHistory
class SimplexHistory {
public:
    void add_simplex(Simplex* simplex);
    std::vector<Simplex*> get_vector_history();
    ~SimplexHistory();
};

// Класс ExpressionTree
class ExpressionTree : public IFunction {
public:
    ~ExpressionTree();
    static ExpressionTree* create_tree(const std::string& function_str);
    double evaluate(const IPoint* variables = nullptr) const override;
    std::string json_tree();
    int get_number_variables() override;
};

// Класс NelderMeadMethod
class NelderMeadMethod {
public:
    NelderMeadMethod(IFunction* function_, double reflection_ = 1.0,
                   double expansion_ = 2.0, double contraction_ = 0.5,
                   double homothety_ = 0.5, double dispersion_ = 0.0001);
    
    void set_simplex(const Simplex* simplex_);
    SimplexHistory* minimum_search(int number_steps = 10000);
    ~NelderMeadMethod();
};

// Указание на то, что C# должен освобождать память для этих классов
%newobject IPoint::create_point;
%newobject IPoint::clone;
%newobject Point::clone;
%newobject Simplex::create_simplex;
%newobject Simplex::centroid;
%newobject Simplex::clone;
%newobject Simplex::get_vertex;
%newobject ExpressionTree::create_tree;
%newobject NelderMeadMethod::minimum_search;

// Отключаем сложные операции с векторами
%ignore std::vector<IPoint*>::reserve;
%ignore std::vector<IPoint*>::insert;
%ignore std::vector<IPoint*>::erase;
%ignore std::vector<Simplex*>::reserve;
%ignore std::vector<Simplex*>::insert;
%ignore std::vector<Simplex*>::erase;