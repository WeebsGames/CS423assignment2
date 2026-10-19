# Locate the C# compiler: use CSC if given (make CSC=...), else csc.exe on PATH,
# else the newest .NET Framework csc.exe under the Windows directory.
WINDOWS_DIR := $(subst \,/,$(or $(SystemRoot),$(SYSTEMROOT),$(windir),$(WINDIR),C:/Windows))
ifeq ($(origin CSC),undefined)
  ifneq ($(strip $(shell where csc.exe 2>NUL)),)
    CSC := csc.exe
  else
    CSC := $(lastword $(sort $(wildcard $(WINDOWS_DIR)/Microsoft.NET/Framework/v*/csc.exe)))
    CSC64 := $(lastword $(sort $(wildcard $(WINDOWS_DIR)/Microsoft.NET/Framework64/v*/csc.exe)))
    CSC := $(or $(CSC64),$(CSC))
  endif
endif
ifeq ($(strip $(CSC)),)
  $(error Could not find csc.exe. Install the .NET Framework or run: make CSC="path\to\csc.exe")
endif

all: pdollar

pdollar: pdollar.cs
	"$(CSC)" /out:Pdollar.exe pdollar.cs

run: Pdollar.exe
	.\Pdollar.exe

clean:
	del /Q Pdollar.exe

.PHONY: clean run