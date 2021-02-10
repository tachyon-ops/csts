	.section	__TEXT,__text,regular,pure_instructions
	.build_version macos, 11, 0
	.section	__TEXT,__literal8,8byte_literals
	.p2align	3                               ## -- Begin function fib
LCPI0_0:
	.quad	0x3ff0000000000000              ## double 1
LCPI0_1:
	.quad	0x4008000000000000              ## double 3
LCPI0_2:
	.quad	0xbff0000000000000              ## double -1
LCPI0_3:
	.quad	0xc000000000000000              ## double -2
	.section	__TEXT,__text,regular,pure_instructions
	.globl	_fib
	.p2align	4, 0x90
_fib:                                   ## @fib
	.cfi_startproc
## %bb.0:                               ## %entry
	subq	$24, %rsp
	.cfi_def_cfa_offset 32
	ucomisd	LCPI0_1(%rip), %xmm0
	jae	LBB0_2
## %bb.1:
	movsd	LCPI0_0(%rip), %xmm0            ## xmm0 = mem[0],zero
	addq	$24, %rsp
	retq
LBB0_2:                                 ## %else
	movapd	%xmm0, %xmm1
	movsd	LCPI0_2(%rip), %xmm0            ## xmm0 = mem[0],zero
	addsd	%xmm1, %xmm0
	movsd	%xmm1, 8(%rsp)                  ## 8-byte Spill
	callq	_fib
	movsd	%xmm0, 16(%rsp)                 ## 8-byte Spill
	movsd	8(%rsp), %xmm0                  ## 8-byte Reload
                                        ## xmm0 = mem[0],zero
	addsd	LCPI0_3(%rip), %xmm0
	callq	_fib
	addsd	16(%rsp), %xmm0                 ## 8-byte Folded Reload
	addq	$24, %rsp
	retq
	.cfi_endproc
                                        ## -- End function
	.globl	_main                           ## -- Begin function main
	.p2align	4, 0x90
_main:                                  ## @main
	.cfi_startproc
## %bb.0:                               ## %entry
	pushq	%rax
	.cfi_def_cfa_offset 16
	callq	_fib
	popq	%rax
	retq
	.cfi_endproc
                                        ## -- End function
.subsections_via_symbols
