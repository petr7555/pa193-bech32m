[![Build and test](https://github.com/petr7555/pa193-bech32m/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/petr7555/pa193-bech32m/actions/workflows/build_and_test.yml)
[![CodeQL](https://github.com/petr7555/pa193-bech32m/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/petr7555/pa193-bech32m/actions/workflows/codeql-analysis.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/7a3625c94f6c483ab9c4c79593693569)](https://www.codacy.com/gh/petr7555/pa193-bech32m/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=petr7555/pa193-bech32m&amp;utm_campaign=Badge_Grade)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/7a3625c94f6c483ab9c4c79593693569)](https://www.codacy.com/gh/petr7555/pa193-bech32m/dashboard?utm_source=github.com&utm_medium=referral&utm_content=petr7555/pa193-bech32m&utm_campaign=Badge_Coverage)

# Bech32m encoder and decoder

## Run released version

- download the latest release `pa193-bech32m-{version}.zip` from https://github.com/petr7555/pa193-bech32m/releases
- unzip `pa193-bech32m-{version}.zip`
- `cd` into the unzipped `pa193-bech32m-{version}` directory
- [make sure you have `dotnet` installed](#install-dotnet)
- `dotnet pa193-bech32m.dll`

## Run from source code

- clone the repository and `cd` into it
- `dotnet run --project pa193-bech32m`
- the command

## Run tests

- `dotnet test`

## Build cross-platform framework-dependent executable

- `dotnet publish -c Release pa193-bech32m`

## Install dotnet

- you need `dotnet` to run this application
- try running `dotnet --version`
- this application has been tested with .NET 5.0
- you can download `dotnet` [here](https://dotnet.microsoft.com/en-us/download)

## Run fuzzer

- this project uses [SharpFuzz](https://github.com/Metalnem/sharpfuzz) for fuzzing
- instrument the assembly by running `dotnet sharpfuzz pa193-bech32m-fuzzer/bin/Debug/net5.0/pa193-bech32m.dll`
- start the fuzzing
  with `afl-fuzz -i pa193-bech32m-fuzzer/testcases -o pa193-bech32m-fuzzer/findings -t 5000 dotnet pa193-bech32m-fuzzer/bin/Debug/net5.0/pa193-bech32m-fuzzer.dll`
