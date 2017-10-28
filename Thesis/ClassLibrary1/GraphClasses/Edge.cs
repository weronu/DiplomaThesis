﻿using Domain.DomainClasses;

namespace Domain.GraphClasses
{
    public class Edge<T> where T : DomainBase
    {
        public Vertex<T> Vertex1 { get; set; }
        public Vertex<T> Vertex2 { get; set; }


        //public Edge(Vertex<T> vertex1, Vertex<T> vertex2)
        //{
        //    Vertex1 = vertex1;
        //    Vertex2 = vertex2;
        //}

        public Edge()
        {
        }
    }
}
