mkdir -p ./TestForParser/wrapper
swig -csharp -c++ -outdir ./TestForParser/wrapper -o ./NelderMead_dll/dll/wrapper.cxx ./Wrapper/expression_tree.i

mkdir -p ./build
cmake -B ./build -S ./NelderMead_dll/dll
cmake --build ./build