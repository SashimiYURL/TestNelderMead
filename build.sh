mkdir ./TestForParser/wrapper_build
swig -csharp -c++ -debug-classes -outdir ./TestForParser/wrapper_build -o ./NelderMead_dll/dll/wrapper.cxx ./Wrapper/expression_tree.i