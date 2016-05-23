﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csMTG
{
    public class mtg : PropertyTree
    {

        #region Attributes

        // Map a vertex to its scale.
        public Dictionary<int, int> scale;

        // Equivalent of parent. Assigns to each vertex a vertex on scale-1.
        public Dictionary<int, int> complex;

        // Equivalent of children. Assigns to each vertex a list of vertices on a lower scale (scale+1).
        public Dictionary<int, List<int>> components;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the MTG.
        /// </summary>
        public mtg()
        {
            // Initialize the attributes

            scale = new Dictionary<int, int>() { { 0, 0 } };
            complex = new Dictionary<int, int>() { };
            components = new Dictionary<int, List<int>>() { };

            // Add default properties

            AddProperty("Edge_Type");
            AddProperty("label");

        }

        #endregion

        #region Querying scale information (Functions: Scales, Scale, NbScales, MaxScale)

        /// <summary>
        /// Returns a list of the different scales of the mtg.
        /// </summary>
        /// <returns> A distinct list of the mtg's scales. </returns>
        public List<int> Scales()
        {
            return scale.Values.Distinct().ToList<int>();
        }

        /// <summary>
        ///  Maps the scale corresponding to the vertex in the parameter.
        /// </summary>
        /// <param name="vertexId"> The vertex identifier. </param>
        /// <returns> The scale to which the vertex belongs. </returns>
        public int? Scale(int vertexId)
        {
            int? returnedScale;

            try
            {
                returnedScale = scale[vertexId];
            }
            catch (KeyNotFoundException)
            {
                returnedScale = null;
            }

            return returnedScale;

        }

        /// <summary>
        /// Counts the number of scales in the mtg.
        /// </summary>
        /// <returns> The number of scales. </returns>
        public int NbScales()
        {
            return Scales().Count;
        }

        /// <summary>
        /// Calculates the highest scale identifier.
        /// </summary>
        /// <returns> The maximum scale. </returns>
        public int MaxScale()
        {
            return scale.Values.Max();
        }

        #endregion

        #region Vertices (Functions: VerticesIterator, Vertices, NbVertices)

        /// <summary>
        /// The set of all vertices is returned.
        /// </summary>
        /// <param name="scale"> Optional. If specified, only vertices
        /// belonging to the scale will be listed. Otherwise, all vertices are listed.</param>
        /// <returns> An iterator of the MTG's vertices according to their scale. </returns>
        IEnumerable<int> VerticesIterator(int scale = -1)
        {
            if (scale < 0)
            {
                foreach (int vid in this.scale.Keys)
                {
                    yield return vid;
                }
            }
            else
            {
                foreach (int vid in this.scale.Keys)
                {
                    if (this.scale[vid] == scale)
                       yield return vid;
                }
            }
        }

        /// <summary>
        /// Returns a list of all vertices in a specific scale.
        /// </summary>
        /// <param name="scale"> Optional parameter. </param>
        /// <returns> A list of the previous function. </returns>
        public List<int> Vertices(int scale = -1)
        {
            return VerticesIterator(scale).ToList();
        }

        /// <summary>
        /// Counts the number of vertices in a scale.
        /// </summary>
        /// <param name="scale"> Optional. If the scale isn't specified, the total number of vertices is counted. </param>
        /// <returns> The number of vertices in a scale. </returns>
        public int NbVertices(int scale = -1)
        {
            if(scale < 0)
                return this.scale.Keys.Count;
            else
                return Vertices(scale).Count;
        }

        #endregion

        #region Edges (Functions: HasVertex, IterEdges, Edges)

        /// <summary>
        /// Verifies that a vertex belongs to the MTG.
        /// </summary>
        /// <param name="vertexId"> The vertex identifier to verify. </param>
        /// <returns> A boolean. </returns>
        public new bool HasVertex(int vertexId)
        {
            return scale.ContainsKey(vertexId);
        }

        /// <summary>
        /// Iterates on the edges of the MTG at a given scale.
        /// </summary>
        /// <param name="scale"> Optional parameter. In case it's not specified, the function iterates on the whole MTG.
        /// If it is specified, it returns an iterator of {parent , child } where the parent belongs to the specified scale. </param>
        /// <returns> An iterator on the MTG's edges. </returns>
        IEnumerable<KeyValuePair<int, int>> IterEdges(int scale = -1)
        {
            
            if (scale < 0)
            {
                foreach (KeyValuePair<int, int> childParent in parent)
                {
                    if (childParent.Value >= 0)
                    {
                        KeyValuePair<int, int> edges = new KeyValuePair<int, int>(childParent.Value, childParent.Key);

                        yield return edges;

                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, int> childParent in parent)
                {
                    if (childParent.Value >= 0)
                    {
                        if (this.scale[childParent.Value] == scale)
                        {
                            KeyValuePair<int, int> edges = new KeyValuePair<int, int>(childParent.Value, childParent.Key);

                            yield return edges;
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// Returns a list of edges at a scale.
        /// </summary>
        /// <param name="scale"> Optional. If it's specified, it returns all parents which belong to the scale and their children.</param>
        /// <returns> A list of edges at a scale. </returns>
        public List<KeyValuePair<int, int>> Edges(int scale = -1)
        {
            return IterEdges(scale).ToList();
        }

        #endregion

        #region Roots (Functions: RootsIterator, Roots)

        /// <summary>
        /// Returns an iterator of the roots of the tree graphs at a given scale.
        /// </summary>
        /// <param name="scale"> The specified scale (Optional). If not specified, all roots of the MTG will be listed. </param>
        /// <returns> iterator on vertex identifiers of root vertices at a given scale. </returns>
        IEnumerable<int> RootsIterator(int scale = 0)
        {
            foreach (int vertexId in Vertices(scale))
            {
                if (Parent(vertexId) == -1)
                    yield return vertexId;
            }
        }

        /// <summary>
        /// Returns a list of the roots of the tree graphs at a given scale.
        /// </summary>
        /// <param name="scale"> The specified scale (Optional). If not specified, all MTG's roots will be listed. </param>
        /// <returns> List on vertex identifiers of root vertices at a given scale. </returns>
        public List<int> Roots(int scale = 0)
        {
            return RootsIterator(scale).ToList();
        }

        #endregion

        #region Complex

        /// <summary>
        /// Gets the complex value for a key.
        /// </summary>
        /// <param name="vertexId"> Vertex identifier.</param>
        /// <returns> Returns the vertex's complex. If it doesn't have a complex, it returns null. </returns>
        int? GetComplex(int vertexId)
        {
            if (complex.ContainsKey(vertexId))
                return complex[vertexId];
            else
                return null;
        }

        /// <summary>
        /// Returns the complex of a vertex.
        /// </summary>
        /// <param name="vertexId"> Vertex identifier. </param>
        /// <returns> The complex of the vertex or null. </returns>
        public int? Complex(int vertexId)
        {
            if (HasVertex(vertexId))
            {
                int? complexId = GetComplex(vertexId);

                while (complexId == null)
                {
                    vertexId = (int)Parent(vertexId);
                    if(vertexId == -1)
                        break;
                    complexId = GetComplex(vertexId);
                }

                return complexId;
            }
            else
                return null;
        }

        /// <summary>
        /// Returns the complex of a vertex at a specific scale.
        /// </summary>
        /// <param name="vertexId"> Vertex identifier. </param>
        /// <param name="scale"> Scale. </param>
        /// <returns> The complex of a vertex at a scale. </returns>
        public int ComplexAtScale(int vertexId, int scale)
        {
            int complexId = vertexId;
            int? currentScale = Scale(complexId);

            if (currentScale != null)
            {
                for (int i = scale; i < currentScale; i++)
                {
                    complexId = (int)Complex(complexId);
                }
                return complexId;
            }
            else
                throw new ArgumentException("This vertex does not exist.");
        }

        #endregion

        #region Components (Functions: ComponentsRootsIter, ComponentsIterator, Components, NbComponents)

        /// <summary>
        /// For the vertex in question, go over the tree graphs that compose it
        /// and return their roots.
        /// </summary>
        /// <param name="vertexId"> Vertex identifier. </param>
        /// <returns> Iterator over the roots of the trees that compose the vertex. </returns>
        IEnumerable<int> ComponentRootsIter(int vertexId)
        {
            List<int> components = this.components[vertexId];

            foreach (int ci in components)
            {
                int p = (int)Parent(ci);
                if (p == -1 || Complex(p) != vertexId)
                    yield return ci;
            }

        }

        /// <summary>
        /// Iterate the components of a vertex.
        /// </summary>
        /// <param name="vertexId"> Vertex identifier. </param>
        /// <returns> Iterator over the components. </returns>
        IEnumerable<int> ComponentsIterator(int vertexId)
        {
            traversal t = new traversal();

            if (components.ContainsKey(vertexId))
            {
                foreach (int v in ComponentRootsIter(vertexId))
                {
                    foreach (int vertex in t.IterativePostOrder(this, v, vertexId))
                    {
                        yield return vertex;
                    }
                }
            }
        }

        /// <summary>
        /// Lists the components of a vertex.
        /// </summary>
        /// <param name="vertexId"> Vertex identifier. </param>
        /// <returns> List of components. </returns>
        public List<int> Components(int vertexId)
        {
            return ComponentsIterator(vertexId).ToList();
        }

        /// <summary>
        /// Counts the number of components for a specific vertex.
        /// </summary>
        /// <param name="vertexId"> Vertex identifier. </param>
        /// <returns> Number of vertex's components. </returns>
        public int NbComponents(int vertexId)
        {
            return Components(vertexId).Count;
        }



        #endregion


        static void Main(String[] args)
        {

        }
    }
}

