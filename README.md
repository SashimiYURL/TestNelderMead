# NelderMead Testing App

**NelderMead Testing** is a console application on C# which runs tests on the main backend [library](https://github.com/sdanils/NelderMead_dll)

## Dependencies

- swig - `4.3.0+`
- dotnet - `9.0`

## How to run

To launch the application you need to follow steps wrutten below:

### Build backend

First of all build backend using bash script

```bash
./build.sh
```

### Run tests

Now you are able to run all tests
You can see test results after running dotnet command

```bash
dotnet test
```

## Also

If you are interested, also check similar to NelderMead project:

- [NelderMead_dll](https://github.com/sdanils/NelderMead_dll) - NelderMead backend on C++
- [NelderMead_front](https://github.com/Sergho/NelderMead-front) - NelderMead frontend app on NodeJS
