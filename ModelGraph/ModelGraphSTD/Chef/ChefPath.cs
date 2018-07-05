using System;
using System.Collections.Generic;
using System.Xml;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        #region CreateQueryPaths  =============================================
        internal bool TryCreateQueryPaths(Graph g)
        {
            if (g.PathQuerys.Count == 0) return false;
            var L = new Level(g);
            foreach (var (q1, q2) in g.PathQuerys)
            {
                L.Add(new QueryPath(g, q1, q2));
            }
            return (L.Count > 0);
        }
        #endregion

        #region TryPathReduction  =============================================
        internal bool TryPathReduction(Graph G)
        {
            if (!G.TryGetTopLevel(out Level L)) return false;

            var inputPaths = L.Paths.ToArray(); // copy of this levels path list
            var N = inputPaths.Length;
            if (N < 2) return false;

            var nodeIndex = new Dictionary<Item, List<int>>(N * 2); // index into inputPaths 

            var nodeList1 = new List<Item>(N); // list of nodes with 1 connection
            var nodeList2 = new List<Item>(N); // list of nodew with 2 connections

            bool found, found2, found3 = false;

            #region build the nodeIndex dictionary  ===========================
            for (int i = 0; i < N; i++)
            {
                var path = inputPaths[i];


                var head = path.Head;
                if (!nodeIndex.TryGetValue(head, out List<int> indexList)) nodeIndex.Add(head, (indexList = new List<int>(4)));
                indexList.Add(i);

                var tail = path.Tail;
                if (!nodeIndex.TryGetValue(tail, out indexList)) nodeIndex.Add(tail, (indexList = new List<int>(4)));
                indexList.Add(i);
            }
            #endregion

            #region lists of nodes that have exactly 1 or exactly 2 connections
            foreach (var e in nodeIndex)
            {
                if (e.Value.Count == 1)
                {
                    nodeList1.Add(e.Key);

                    var path = inputPaths[e.Value[0]];
                    if (path.Head == e.Key) path.Reverse();
                    path.IsRadial = true;
                }
                else if (e.Value.Count == 2)
                {
                    nodeList2.Add(e.Key);
                }
            }
            #endregion

            #region seed and grow the flaredPath lists  =======================
            var flaredPaths = new List<List<Path>>(nodeList1.Count);

            foreach (var node in nodeList1)
            {
                var i = nodeIndex[node][0];
                var path = inputPaths[i];
                if (path == null) continue;

                List<Path> pathList = null;
                foreach (var k in nodeIndex[path.Head])
                {
                    var tp = inputPaths[k];
                    if (tp == null) continue;
                    if (tp == path) continue;
                    if (!tp.IsRadial) continue;

                    if (pathList == null)
                    {
                        pathList = new List<Path>(4)
                        {
                            path
                        };
                        flaredPaths.Add(pathList);
                        inputPaths[i] = null;
                        found3 = true;
                    }
                    pathList.Add(tp);
                    inputPaths[k] = null;
                }
            }
            #endregion

            #region seed and grow the radialPath lists  =======================
            var radialPaths = new List<List<Path>>(nodeList1.Count);

            foreach (var tail in nodeList1)
            {
                List<Path> pathList = null;

                var i = nodeIndex[tail][0];
                var path = inputPaths[i];
                if (path != null)
                {
                    var head = path.Head;

                    found = true;
                    while (found)
                    {
                        found = false;

                        if (nodeIndex[head].Count == 2)
                        {
                            var j = nodeIndex[head][0];
                            var k = nodeIndex[head][1];
                            var path1 = inputPaths[j];
                            var path2 = inputPaths[k];
                            var path3 = (path1 == null || path1 == path) ? path2 : ((path2 == null || path2 == path) ? path1 : null);

                            if (path3 != null)
                            {
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(8);
                                    radialPaths.Add(pathList);
                                    pathList.Add(path);
                                    inputPaths[i] = null;
                                    found3 = true;
                                }
                                found = true;
                                path = path3;
                                if (path.Tail != head) path.Reverse();
                                pathList.Add(path);
                                inputPaths[j] = null;
                                inputPaths[k] = null;
                                head = path.Head;
                            }
                        }
                    }
                }
            }
            #endregion

            #region seed and grow the innerPath lists  ========================
            // innerPaths flow from nodes with 3 or more connections
            found = true;
            var innerPaths = new List<List<Path>>(nodeList2.Count);
            while (found)
            {
                found = false;
                List<Path> pathList = null;

                #region look for a new seed  ==================================
                foreach (var node in nodeList2)
                {
                    var i = nodeIndex[node][0];
                    var j = nodeIndex[node][1];
                    var path1 = inputPaths[i];
                    var path2 = inputPaths[j];
                    if (path1 == null || path2 == null) continue;

                    var h1 = path1.Head;
                    var t1 = path1.Tail;
                    var h2 = path2.Head;
                    var t2 = path2.Tail;
                    if (t1 == h2 && h1 != t2)
                    {
                        if (nodeIndex[h1].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            pathList.Add(path1);
                            pathList.Add(path2);
                            found = found3 = true;
                            break;
                        }
                        else if (nodeIndex[t2].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            path2.Reverse();
                            pathList.Add(path2);
                            path1.Reverse();
                            pathList.Add(path1);
                            found = found3 = true;
                            break;
                        }
                    }
                    else if (t1 == t2 && h1 != h2)
                    {
                        if (nodeIndex[h1].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            pathList.Add(path1);
                            path2.Reverse();
                            pathList.Add(path2);
                            found = found3 = true;
                            break;
                        }
                        else if (nodeIndex[h2].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            pathList.Add(path2);
                            path1.Reverse();
                            pathList.Add(path1);
                            found = found3 = true;
                            break;
                        }
                    }
                    else if (h1 == h2 && t1 != t2)
                    {
                        if (nodeIndex[t1].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            path1.Reverse();
                            pathList.Add(path1);
                            pathList.Add(path2);
                            found = found3 = true;
                            break;
                        }
                        else if (nodeIndex[t2].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            path2.Reverse();
                            pathList.Add(path2);
                            pathList.Add(path1);
                            found = found3 = true;
                            break;
                        }
                    }
                    else if (h1 == t2 && t1 != h2)
                    {
                        if (nodeIndex[t1].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            path1.Reverse();
                            pathList.Add(path1);
                            pathList.Add(path2);
                            found = found3 = true;
                            break;
                        }
                        else if (nodeIndex[h2].Count > 2)
                        {
                            pathList = new List<Path>(4);
                            innerPaths.Add(pathList);
                            inputPaths[i] = null;
                            inputPaths[j] = null;

                            pathList.Add(path2);
                            pathList.Add(path1);
                            found = found3 = true;
                            break;
                        }
                    }
                }
                #endregion

                if (found)
                {
                    found = false;
                    #region grow the innerPath  ===============================
                    found2 = true;
                    while (found2)
                    {
                        found2 = false;
                        var tail = pathList[pathList.Count - 1].Tail;
                        if (nodeIndex[tail].Count > 2) break;

                        foreach (var node in nodeList2)
                        {
                            var i = nodeIndex[node][0];
                            var j = nodeIndex[node][1];
                            var path1 = inputPaths[i];
                            var path2 = inputPaths[j];
                            if (path1 == null)
                            {
                                if (path2 == null) continue;
                                if (path2.Head == tail)
                                {
                                    pathList.Add(path2);
                                    inputPaths[j] = null;
                                    found = found2 = true;
                                    break;
                                }
                                else if (path2.Tail == tail)
                                {
                                    path2.Reverse();
                                    pathList.Add(path2);
                                    inputPaths[j] = null;
                                    found = found2 = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (path2 != null) continue;
                                if (path1.Head == tail)
                                {
                                    pathList.Add(path1);
                                    inputPaths[i] = null;
                                    found = found2 = true;
                                    break;
                                }
                                else if (path1.Tail == tail)
                                {
                                    path1.Reverse();
                                    pathList.Add(path1);
                                    inputPaths[i] = null;
                                    found = found2 = true;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region find all parallel paths  ==================================
            var parallelPaths = new List<List<Path>>(8);

            foreach (var e in nodeIndex)
            {
                List<Path> pathList = null;
                foreach (var i in e.Value)
                {
                    var path1 = inputPaths[i];
                    if (path1 == null) continue;
                    var h1 = path1.Head;
                    var t1 = path1.Tail;
                    if (e.Key == h1)
                    {
                        foreach (var j in nodeIndex[t1])
                        {
                            var path2 = inputPaths[j];
                            if (path2 == null || path2 == path1) continue;
                            var h2 = path2.Head;
                            var t2 = path2.Tail;
                            if (h1 == h2)
                            {
                                if (t1 != t2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4) { path1 };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                            else if (h1 == t2)
                            {
                                if (t1 != h2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4)
                                    {
                                        path1
                                    };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                path2.Reverse();
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                            else if (t1 == h2)
                            {
                                if (h1 != t2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4)
                                    {
                                        path1
                                    };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                path2.Reverse();
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                            else if (t1 == t2)
                            {
                                if (h1 != h2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4)
                                    {
                                        path1
                                    };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                        }
                    }
                    else
                    {
                        foreach (var j in nodeIndex[h1])
                        {
                            var path2 = inputPaths[j];
                            if (path2 == null || path2 == path1) continue;
                            var h2 = path2.Head;
                            var t2 = path2.Tail;
                            if (h1 == h2)
                            {
                                if (t1 != t2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4)
                                    {
                                        path1
                                    };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                            else if (h1 == t2)
                            {
                                if (t1 != h2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4)
                                    {
                                        path1
                                    };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                path2.Reverse();
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                            else if (t1 == h2)
                            {
                                if (h1 != t2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4)
                                    {
                                        path1
                                    };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                path2.Reverse();
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                            else if (t1 == t2)
                            {
                                if (h1 != h2) continue;
                                if (pathList == null)
                                {
                                    pathList = new List<Path>(4)
                                    {
                                        path1
                                    };
                                    inputPaths[i] = null;
                                    parallelPaths.Add(pathList);
                                    found3 = true;
                                }
                                pathList.Add(path2);
                                inputPaths[j] = null;
                            }
                        }
                    }
                }
            }
            #endregion

            if (!found3) return false;

            #region create and populate next path level  ======================
            L = new Level(G);

            foreach (var path in inputPaths)
            {
                if (path == null) continue;
                L.Add(path);
            }

            foreach (var list in radialPaths)
            {
                list.Reverse();
                if (list.Count > 1)
                    L.Add(new SeriesPath(G, list.ToArray(), true));
                else if (list.Count == 1 && list[0] != null)
                    L.Add(list[0]);

            }

            foreach (var list in innerPaths)
            {
                if (list.Count > 1)
                    L.Add(new SeriesPath(G, list.ToArray()));
                else if (list.Count == 1 && list[0] != null)
                    L.Add(list[0]);
            }

            foreach (var list in parallelPaths)
            {
                if (list.Count < 2) continue;
                L.Add(new ParallelPath(G, list.ToArray(), list[0].IsRadial));
            }

            foreach (var list in flaredPaths)
            {
                if (list.Count < 2) continue;
                L.Add(new FlaredPath(G, list.ToArray()));
            }
            #endregion

            return true;
        }
        #endregion

    }
}
