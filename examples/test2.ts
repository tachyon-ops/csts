var a: number = 3;
println(a);
a = 2;
println(a);

function f(): void {
	println("a" + "b");
	println("a" + 3);

	if (true && false) {
		println("if true && false");
	} else {
		println("else true && false");
	}

	if (true || false) {
		println("if true || false");
	} else {
		println("else true || false");
	}
}
f();

