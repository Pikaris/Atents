using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSort : IComparable<TestSort>
{
    public int a;
    public float b;
    public string c;

    public TestSort(int a, float b, string c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    // 기본적으로 a를 기준으로 정렬
    // 같음의 기준은 a를 기준으로 판단

    public int CompareTo(TestSort other)
    {
        if (other == null) return -1;

        return a.CompareTo(other.a);
    }


    public static bool operator == (TestSort left, TestSort right)
    {
        return left.a == right.a;
    }

    public static bool operator != (TestSort left, TestSort right)
    {
        return left.a != right.a;
    }

    public override bool Equals(object obj)
    {
        return obj is TestSort sort && a == sort.a;
    }

    public override int GetHashCode()
    {
        return a.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
