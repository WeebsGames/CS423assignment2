CSC = csc.exe

all: pdollar

pdollar: pdollar.cs
	"$(CSC)" /out:Pdollar.exe pdollar.cs

run: Pdollar.exe
	.\Pdollar.exe

clean:
	del /Q Pdollar.exe

.PHONY: clean run