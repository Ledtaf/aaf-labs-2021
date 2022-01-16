using System;
using System.Collections.Generic;
using System.Linq;

namespace RBush
{
	public partial class RBush<T>
	{
		public class Node : ISpatialData
		{
			private Envelope _envelope;

			internal Node(List<ISpatialData> items, int height)
			{
				this.Height = height;
				this.children = items;
				ResetEnvelope();
			}

			internal void Add(ISpatialData node)
			{
				children.Add(node);
				_envelope = Envelope.Extend(node.Envelope);
			}

			internal string GetData() => _envelope.ToString();

			internal void Remove(ISpatialData node)
			{
				children.Remove(node);
				ResetEnvelope();
			}

			internal void RemoveRange(int index, int count)
			{
				children.RemoveRange(index, count);
				ResetEnvelope();
			}

			internal void ResetEnvelope()
			{
				_envelope = GetEnclosingEnvelope(children);
			}

			internal readonly List<ISpatialData> children;

			public IReadOnlyList<ISpatialData> Children => children;
			public int Height { get; }
			public bool IsLeaf => Height == 1;
			public ref readonly Envelope Envelope => ref _envelope;

			#region print
			internal void print(ref System.Text.StringBuilder buffer, string prefix, string childrenPrefix)
			{
				Queue<Node> remainingChildren;
				Node item;
				
				if(!this.IsLeaf)
				{
					remainingChildren = new Queue<Node>(this.children.Cast<Node>().ToArray());
					if(remainingChildren.Count() != 0)
					{
						item = remainingChildren.Dequeue();
						while(true)
						{
							if(remainingChildren.Count() <= 0) item.print(ref buffer, childrenPrefix + "└── ", childrenPrefix + "    "); //?
							else if(remainingChildren.Count() > 0) item.print(ref buffer, childrenPrefix + "├── ", childrenPrefix + "│   ");
							if(remainingChildren.Count() <= 0) break;
							item = remainingChildren.Dequeue();
						}
					}
				}
				else if (this.IsLeaf)
				{
					int index = this.children.Count() - 1;
					foreach(var sect in this.children.Cast<T>()
											.OrderByDescending(t => t.Envelope.MaxX - t.Envelope.MinX)
											.ThenByDescending(t => t.Envelope.MinX)
											.ThenByDescending(t => t.Envelope.MaxX))	
					{
						if(index == 1)
						{
							buffer.Append(prefix);
							buffer.Append(sect.ToString());
							buffer.Append('\n');

							prefix = childrenPrefix + "└── ";
							
						}
						else
						{
							buffer.Append(prefix);
							buffer.Append(sect.ToString());
							buffer.Append('\n');

							prefix = childrenPrefix + "├── ";
							
						}
						index--;
					}
				}
			}
			#endregion
		}
	}
}
