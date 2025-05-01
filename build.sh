mkdir -p ./TestForParser/wrapper_build
swig -csharp -c++ -outdir ./TestForParser/wrapper_build -o ./NelderMead_dll/dll/wrapper.cxx ./Wrapper/expression_tree.i

mkdir -p ./build
cmake -B ./build -S ./NelderMead_dll/dll
cmake --build ./build