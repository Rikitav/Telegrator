using System;
public class TestAttr : Attribute { public int Val; public TestAttr(int x) {} }
[TestAttr(1, Val = 2)]
public class C {}
