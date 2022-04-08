[![Build and test](https://github.com/petr7555/pa193-bech32m/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/petr7555/pa193-bech32m/actions/workflows/build_and_test.yml)
[![CodeQL](https://github.com/petr7555/pa193-bech32m/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/petr7555/pa193-bech32m/actions/workflows/codeql-analysis.yml)
[![Fuzzer](https://github.com/petr7555/pa193-bech32m/actions/workflows/fuzzer.yml/badge.svg)](https://github.com/petr7555/pa193-bech32m/actions/workflows/fuzzer.yml)
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

## Run tests

- `dotnet test`

## Build cross-platform framework-dependent executable

- `dotnet publish -c Release pa193-bech32m`

## Run fuzzer

- this project uses [SharpFuzz](https://github.com/Metalnem/sharpfuzz) for fuzzing
- you need to [install afl](#install-afl)
- build the project with `dotnet build pa193-bech32m-fuzzer`
- instrument the assembly by running `dotnet sharpfuzz pa193-bech32m-fuzzer/bin/Debug/net5.0/pa193-bech32m.dll`
- start the fuzzing with
  `afl-fuzz -i pa193-bech32m-fuzzer/testcases -o pa193-bech32m-fuzzer/findings -t 5000 dotnet pa193-bech32m-fuzzer/bin/Debug/net5.0/pa193-bech32m-fuzzer.dll FUZZ_TEST_NAME`
  where `FUZZ_TEST_NAME` is one of the public static methods available in `pa193-bech32m-fuzzer/Fuzzer.cs`
  (e.g. _FuzzData_)

## Install dotnet

- you need `dotnet` to run this application
- try running `dotnet --version`
- this application has been tested with .NET 5.0
- you can download `dotnet` [here](https://dotnet.microsoft.com/en-us/download)

## Install afl

- instructions taken from https://github.com/Metalnem/sharpfuzz#installation
  ```bash
  # Download and extract the latest afl-fuzz source package
  wget http://lcamtuf.coredump.cx/afl/releases/afl-latest.tgz
  tar -xvf afl-latest.tgz
  
  rm afl-latest.tgz
  cd afl-2.52b/
  
  # Patch afl-fuzz so that it doesn't check whether the binary
  # being fuzzed is instrumented (we have to do this because
  # we are going to run our programs with the dotnet run command,
  # and the dotnet binary would fail this check)
  wget https://github.com/Metalnem/sharpfuzz/raw/master/patches/RemoveInstrumentationCheck.diff
  patch < RemoveInstrumentationCheck.diff
  
  # Install afl-fuzz
  make install
  cd ..
  rm -rf afl-2.52b/
  ```
