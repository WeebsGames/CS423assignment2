CONFIG ?= Release
OUT_DIR := bin
EXE := $(OUT_DIR)\Pdollar.exe

ifeq ($(strip $(shell where dotnet 2>NUL)),)
  $(error Could not find dotnet. Install the .NET SDK from https://dotnet.microsoft.com/download)
endif

all: pdollar

pdollar: pdollar.cs Pdollar.csproj
	dotnet build Pdollar.csproj -c $(CONFIG) -o $(OUT_DIR) --nologo

# Pass program arguments with: make run ARGS="..."
run: pdollar
	$(EXE) $(ARGS)

clean:
	dotnet clean Pdollar.csproj -c $(CONFIG) -o $(OUT_DIR) --nologo
	if exist $(OUT_DIR) rmdir /S /Q $(OUT_DIR)
	if exist obj rmdir /S /Q obj

.PHONY: all pdollar run cleanar.exe

.PHONY: clean run