using System;
using System.Collections.Generic;
using System.Linq;

using ArgumentException = System.ArgumentException;

namespace Generics.Tables
{
    public class Cell<T1, T2, T3>
    {
        public T1 Row;
        public T2 Column;
        public T3 Value;

        public Cell(T1 row)
        {
            Row = row;
        }

        public Cell(T2 column)
        {
            Column = column;
        }

        public Cell(T1 row, T2 column, T3 value)
        {
            Row = row;
            Column = column;
            Value = value;
        }
    }

    public class Indexer<T1, T2, T3>
    {
        public Indexer(Table<T1, T2, T3> table)
        {
            _table = table;
        }

        private readonly Table<T1, T2, T3> _table;

        public T3 this[T1 x, T2 y]
        {
            get
            {
                switch (this)
                {
                    case OpenTableIndexer<T1, T2, T3> _:
                        return _table.Cells.FirstOrDefault(c =>
                            c.Row != null && c.Row.Equals(x) && c.Column != null && c.Column.Equals(y)) == null
                            ? default
                            : _table.Cells.First(c => c.Row.Equals(x) && c.Column.Equals(y)).Value;
                    case ExistedTableIndexer<T1, T2, T3> _:
                        return _table.Cells.FirstOrDefault(c =>
                            c.Row != null && c.Row.Equals(x) && c.Column != null && c.Column.Equals(y)) == null
                            ? throw new ArgumentException()
                            : _table.Cells.First(c => c.Row.Equals(x) && c.Column.Equals(y)).Value;
                }
                throw new ArgumentNullException();
            }
            set
            {
                var cell = _table.Cells.FirstOrDefault(c =>
                    c.Row != null && c.Row.Equals(x) && c.Column != null && c.Column.Equals(y));
                if (cell == null)
                {
                    switch (this)
                    {
                        case OpenTableIndexer<T1, T2, T3> _:
                            _table.Cells.Add(new Cell<T1, T2, T3>(x, y, value));
                            _table.Rows.Add(x);
                            _table.Columns.Add(y);
                            break;
                        case ExistedTableIndexer<T1, T2, T3> _:
                            throw new ArgumentException();
                    }
                }
                else cell.Value = value;
            }
        }
    }

    public class OpenTableIndexer<T1, T2, T3> : Indexer<T1, T2, T3>
    {
        public OpenTableIndexer(Table<T1, T2, T3> table) : base(table)
        {
        }
    }

    public class ExistedTableIndexer<T1, T2, T3> : Indexer<T1, T2, T3>
    {
        public ExistedTableIndexer(Table<T1, T2, T3> table) : base(table)
        {
        }
    }

    public class Table<T1, T2, T3>
    {
        public Table()
        {
            Open = new OpenTableIndexer<T1, T2, T3>(this);
            Existed = new ExistedTableIndexer<T1, T2, T3>(this);
        }

        public HashSet<T1> Rows { get; } = new HashSet<T1>();
        public HashSet<T2> Columns { get; } = new HashSet<T2>();
        public HashSet<Cell<T1, T2, T3>> Cells { get; } = new HashSet<Cell<T1, T2, T3>>();
        public Indexer<T1, T2, T3> Open { get; }
        public Indexer<T1, T2, T3> Existed { get; }

        public void AddRow(T1 row)
        {
            Rows.Add(row);
            if (Cells.Count != 0 && !Cells.Last().Column.Equals(default(T2)))
            {
                Cells.Last().Row = row;
                return;
            }
            Cells.Add(new Cell<T1, T2, T3>(row));
        }

        public void AddColumn(T2 column)
        {
            Columns.Add(column);
            if (Cells.Count != 0 && !Cells.Last().Row.Equals(default(T1)))
            {
                Cells.Last().Column = column;
                return;
            }
            Cells.Add(new Cell<T1, T2, T3>(column));
        }
    }
}