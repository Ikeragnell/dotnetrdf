using VDS.RDF;
using VDS.RDF.Query.Datasets;
using System;

using VDS.RDF.Query;

using VDS.RDF.Parsing;


namespace VDS.RDF.Shacl
{
    internal class DatasetWrapperGraph : Graph
    {
        internal InMemoryDataset Dataset { get; }
        internal IRefNode GraphName { get; }

        internal DatasetWrapperGraph(InMemoryDataset dataset)
        {
            Dataset = dataset;
        }

        internal DatasetWrapperGraph(InMemoryDataset dataset, IRefNode graphName) : this(dataset)
        {
            GraphName = graphName;
            if (graphName.Equals(VocabularyDs.TargetGraphDefault))
                GraphName = null;

            IGraph g = dataset[GraphName];
            this.Assert(g.Triples);
            SetAsDefaultGraph();
        }

        internal DatasetWrapperGraph(InMemoryDataset dataset, IGraph graph) : this(dataset)
        {
            GraphName = new BlankNode("combinedGraph");

            var combinedGraph = new Graph(GraphName);
            foreach (var triple in graph.Triples)
            {
                combinedGraph.Assert(triple);
            }
            Dataset.AddGraph(combinedGraph);
            this.Assert(graph.Triples);
            SetAsDefaultGraph();
        }

        private void SetAsDefaultGraph()
        {

            // If already default graph do nothing
            if (GraphName == null)
                return;

            IGraph namedGraph = Dataset[GraphName];
            IGraph defaultGraph = Dataset[(IRefNode)null];

            var namedAsDefaultGraph = new Graph();
            var defaultAsNamedGraph = new Graph(VocabularyDs.TargetGraphDefault.Uri);

            foreach (var triple in namedGraph.Triples)
            {
                namedAsDefaultGraph.Assert(triple);
            }
            foreach (var triple in defaultGraph.Triples)
            {
                defaultAsNamedGraph.Assert(triple);
            }

            Dataset.RemoveGraph((IRefNode)null);
            Dataset.RemoveGraph(GraphName);

            Dataset.AddGraph(defaultAsNamedGraph);
            Dataset.AddGraph(namedAsDefaultGraph);
        }

        internal void ResetDefaultGraph()
        {
            // If already default graph do nothing
            if (GraphName == null)
                return;

            IGraph defaultAsNamedGraph = Dataset[(IRefNode)VocabularyDs.TargetGraphDefault];
            IGraph namedAsDefaultGraph = Dataset[(IRefNode)null];

            var defaultGraph = new Graph();
            var namedGraph = new Graph(GraphName);

            foreach (var triple in defaultAsNamedGraph.Triples)
            {
                defaultGraph.Assert(triple);
            }
            foreach (var triple in namedAsDefaultGraph.Triples)
            {
                namedGraph.Assert(triple);
            }

            Dataset.RemoveGraph((IRefNode)VocabularyDs.TargetGraphDefault);
            Dataset.RemoveGraph((IRefNode)null);

            Dataset.AddGraph(defaultGraph);

            if (GraphName is not BlankNode){
                Dataset.AddGraph(namedGraph);
            }
        }
    }
}