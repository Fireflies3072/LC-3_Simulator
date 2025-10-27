## LC-3 Simulator

An educational LC-3 computer simulator with a modern GUI. Load LC-3 object/assembly, inspect registers and memory, and step through execution with a clean, keyboard-friendly interface.

### Why this project?
- **Modern GUI**: Built with Windows desktop tech for a responsive, accessible experience.
- **Focused on learning**: Clear views of `PC`, `R0–R7`, condition codes, and memory so you can see how instructions affect state.

## Getting Started
- Clone the repo and open the solution `LC-3_Simulator.sln` in Visual Studio 2022 (or build with the `dotnet` CLI).
- Build and run the `LC-3_Simulator` project.
- Open or create an LC-3 program, then step instructions to observe state changes.

**Main interface**

![](https://fireflies3072.blob.core.windows.net/blog/images/2023-11-lc3-simulator/gui1.jpg)

**Write code file**

![](https://fireflies3072.blob.core.windows.net/blog/images/2023-11-lc3-simulator/gui2.jpg)

**Step over each instruction**

![](https://fireflies3072.blob.core.windows.net/blog/images/2023-11-lc3-simulator/gui3.jpg)

## LC-3 Instruction Set Overview
Short, practical descriptions of the core LC-3 instructions. For brevity, `SEXT` means sign-extend and all arithmetic is 16‑bit.

### Arithmetic and Logic
- **ADD DR, SR1, SR2 | imm5**: `DR ← SR1 + (SR2 | SEXT(imm5))`, updates condition codes `(N/Z/P)`.
- **AND DR, SR1, SR2 | imm5**: `DR ← SR1 & (SR2 | SEXT(imm5))`, updates condition codes.
- **NOT DR, SR**: Bitwise complement. `DR ← NOT SR`, updates condition codes.

### Loads (copy from memory into a register)
- **LD DR, label (PC-relative)**: `DR ← mem[PC + SEXT(PCoffset9)]`, updates condition codes.
- **LDI DR, label (indirect)**: `DR ← mem[mem[PC + SEXT(PCoffset9)]]`, updates condition codes.
- **LDR DR, BaseR, offset6 (base+offset)**: `DR ← mem[BaseR + SEXT(offset6)]`, updates condition codes.
- **LEA DR, label (address only)**: Load effective address. `DR ← PC + SEXT(PCoffset9)`, updates condition codes.

### Stores (copy from a register into memory)
- **ST SR, label (PC-relative)**: `mem[PC + SEXT(PCoffset9)] ← SR`.
- **STI SR, label (indirect)**: `mem[mem[PC + SEXT(PCoffset9)]] ← SR`.
- **STR SR, BaseR, offset6 (base+offset)**: `mem[BaseR + SEXT(offset6)] ← SR`.

### Control Flow
- **BR[nzp] label**: Conditional branch. If condition codes satisfy the mask `(n/z/p)`, then `PC ← PC + SEXT(PCoffset9)`.
- **JMP BaseR**: Unconditional jump. `PC ← BaseR`.
- **JSR label**: Subroutine call (PC‑relative). `R7 ← PC; PC ← PC + SEXT(PCoffset11)`.
- **JSRR BaseR**: Subroutine call (register). `R7 ← PC; PC ← BaseR`.
- **RET**: Return from subroutine. Alias of `JMP R7`.
- **TRAP trapvect8**: System call. `R7 ← PC; PC ← mem[ZEXT(trapvect8)]`.
- **RTI**: Return from interrupt (privileged; for interrupt handling).

### Condition Codes
Most ALU and load instructions set the `N`, `Z`, and `P` flags based on the result. Branch behavior depends on these flags.

## Current Status and Limitations
- **Memory only**: Basic memory operations are supported. **I/O and interrupts are not yet supported**.
- **Single-step only**: You can step one instruction at a time. **Continuous run can cause the UI to stop responding**.
- **Windows-only**: Currently targets Windows desktop.

## Future Plan

- **I/O and TRAP services**: Keyboard/console devices and user-visible TRAP routines.
- **Interrupts**: Proper interrupt entry/return, including `RTI` paths.
- **Stable continuous run**: A safe and responsive run/stop loop with throttling and breakpoints.
- **Cross-platform**: Move toward a cross-platform UI to support macOS and Linux.
- **Debugger ergonomics**: Breakpoints, watch variables, and better memory inspectors.

## Contributing
Issues and pull requests are welcome—especially around correctness, UI/UX, and test programs that validate instruction semantics.

## Educational Note
LC-3 is designed for learning and is not used in production systems. This simulator aims to make the architecture easier to explore by visualizing state changes as you step through real code.
