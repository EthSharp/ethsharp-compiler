using System;
using System.Collections.Generic;
using EthSharp.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EthSharp.Tests.Compiler
{
    public class EthSharpAllowedTypesVisitorTests
    {
        [Fact]
        public void can_use_allowed_types()
        {
            var sut = new EthSharpAllowedTypesVisitor(new HashSet<string> {typeof(string).Name});

            var tree = CSharpSyntaxTree.ParseText(@"
using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    private String y {get; set;}
    private String z;

    public String Test2(String parameter)
    {
        String x;
        return Test() + ""1"";
    }

    private String Test()
    {
        return ""1"";
    }
}");

            sut.Visit(tree.GetRoot());
        }

        [Fact]        
        public void can_reject_not_allowed_type_in_method_return()
        {
            var sut = new EthSharpAllowedTypesVisitor(new HashSet<string> { typeof(string).Name });

            var tree = CSharpSyntaxTree.ParseText(@"
using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    public UInt256 Test2(String parameter)
    {
        String x;
        return Test() + ""1"";
    }

    private String Test()
    {
        return ""1"";
    }
}");
            Assert.Throws<Exception>(() => sut.Visit(tree.GetRoot()));

        }

        [Fact]
        public void can_reject_not_allowed_type_in_method_parameter()
        {
            var sut = new EthSharpAllowedTypesVisitor(new HashSet<string> { typeof(string).Name });

            var tree = CSharpSyntaxTree.ParseText(@"
using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    public String Test2(UInt256 parameter)
    {
        String x;
        return Test() + ""1"";
    }

    private String Test()
    {
        return ""1"";
    }
}");
            Assert.Throws<Exception>(() => sut.Visit(tree.GetRoot()));

        }

        [Fact]
        public void can_reject_not_allowed_type_in_local_variable()
        {
            var sut = new EthSharpAllowedTypesVisitor(new HashSet<string> { typeof(string).Name });

            var tree = CSharpSyntaxTree.ParseText(@"
using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    public String Test2(String parameter)
    {
        UInt256 x;
        return Test() + ""1"";
    }

    private String Test()
    {
        return ""1"";
    }
}");
            Assert.Throws<Exception>(() => sut.Visit(tree.GetRoot()));

        }

        [Fact]
        public void can_reject_not_allowed_type_in_member_declaration()
        {
            var sut = new EthSharpAllowedTypesVisitor(new HashSet<string> { typeof(string).Name });

            var tree = CSharpSyntaxTree.ParseText(@"
using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    private UInt256 y;

    public String Test2(String parameter)
    {
        String x;
        return Test() + ""1"";
    }

    private String Test()
    {
        return ""1"";
    }
}");
            Assert.Throws<Exception>(() => sut.Visit(tree.GetRoot()));

        }

        [Fact]
        public void can_reject_not_allowed_type_in_property_declaration()
        {
            var sut = new EthSharpAllowedTypesVisitor(new HashSet<string> { typeof(string).Name });

            var tree = CSharpSyntaxTree.ParseText(@"
using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    private UInt256 y {get; set;}

    public String Test2(String parameter)
    {
        String x;
        return Test() + ""1"";
    }

    private String Test()
    {
        return ""1"";
    }
}");
            Assert.Throws<Exception>(() => sut.Visit(tree.GetRoot()));

        }
    }
}