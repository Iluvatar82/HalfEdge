﻿using HalfEdge.Models;

namespace HalfEdge.Tests.Models
{
    public class HalfEdge
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void CreateHalfEdge_Test()
        {
            var vertexStart = new Vertex<double>(0, 0, 0);
            var vertexEnd = new Vertex<double>(1, 1, 1);

            var halfEdge = new HalfEdge<double>(vertexStart, vertexEnd);
            
            
            Assert.Multiple(() =>
            {
                Assert.That(halfEdge.Start, Is.EqualTo(vertexStart));
                Assert.That(halfEdge.End, Is.EqualTo(vertexEnd));
                Assert.That(halfEdge.Opposite, Is.Null);
                Assert.That(halfEdge.Start.OutHalfEdges, Has.Count.EqualTo(1));
                Assert.That(halfEdge.Start.OutHalfEdges[0], Is.EqualTo(halfEdge));
                Assert.That(halfEdge.End.OutHalfEdges, Is.Empty);
            });
        }

        [Test]
        public void CreateHalfEdge_Opposite_Test()
        {
            var vertexStart = new Vertex<double>(0, 0, 0);
            var vertexEnd = new Vertex<double>(1, 1, 1);

            var halfEdge = new HalfEdge<double>(vertexStart, vertexEnd);
            var halfEdgeOpposite = halfEdge.CreateOpposite();
            
            
            Assert.Multiple(() =>
            {
                Assert.That(halfEdgeOpposite.Start, Is.EqualTo(vertexEnd));
                Assert.That(halfEdgeOpposite.End, Is.EqualTo(vertexStart));
                Assert.That(halfEdgeOpposite.Opposite, Is.EqualTo(halfEdge));
                Assert.That(halfEdgeOpposite.Start.OutHalfEdges, Has.Count.EqualTo(1));
                Assert.That(halfEdgeOpposite.Start.OutHalfEdges[0], Is.EqualTo(halfEdgeOpposite));
                Assert.That(halfEdgeOpposite.End.OutHalfEdges, Has.Count.EqualTo(1));
                Assert.That(halfEdgeOpposite.End.OutHalfEdges[0], Is.EqualTo(halfEdge));
                Assert.That(halfEdgeOpposite.Opposite?.Opposite, Is.EqualTo(halfEdgeOpposite));
            });
        }
    }
}