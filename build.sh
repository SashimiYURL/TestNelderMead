mkdir -p ./TestForParser/wrapper
swig -csharp -c++ -debug-classes -outdir ./TestForParser/wrapper -o ./NelderMead_dll/wrapper.cxx ./Wrapper/NelderMead.i

mkdir -p ./build
cmake -B ./build -S ./NelderMead_dll
cmake --build ./build