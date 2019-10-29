import React, {PropTypes} from 'react';
import styles from './index.css';
import classNames from 'classnames';
import Pagination from 'rc-pagination';
import locale from 'rc-pagination/lib/locale/en_US';

const renderDatum = (datum, headers) => headers.map(h => <td className={getClassName(h)}>{datum[h.key]}</td>);

const getClassName = (header) => styles[header.type] || styles.text;

const isSortable = (header) =>  {
  if (header.sortable)
    return styles.sortable;
};

const getSortIndicator = (sortKey, sortColumn, sortOrder) => {
  if (sortKey !== sortColumn) return;
  return {'Ascending': '▴', 'Descending': '▾'}[sortOrder];
};

const reverseSortOrder = (sortOrder) => ({'Ascending': 'Descending', 'Descending': 'Ascending'}[sortOrder]);

const handleSortClick = (sortKey, sortable, props) => {
  if (!sortable) return;
  const {sortColumn, sortOrder} = props;
  const newSortOrder = sortColumn === sortKey ? reverseSortOrder(sortOrder) : 'Ascending';
  props.onSortChange(sortKey, newSortOrder);
};
export const Table = (props) => <div>
  <table className={classNames(styles.table, props.className)}>
    <thead>
    <tr className={styles.header}>{props.headers.map(h => (
      <th key={h.key} className={classNames(getClassName(h), isSortable(h))}
          onClick={e => handleSortClick(h.sortKey, h.sortable, props)}>{h.name}{getSortIndicator(h.sortKey, props.sortColumn, props.sortOrder)}</th>)
    )}</tr>
    </thead>
    <tbody>{props.data.map(datum => <tr className={styles.row}>{renderDatum(datum, props.headers)}</tr>)}</tbody>
  </table>
  {props.showPagination ?
    <Pagination total={props.totalRecords} current={props.pageNumber} onChange={props.onPageChange}
                pageSize={props.batchSize} locale={locale}/> : ''}
</div>;

Table.propTypes = {
  className: PropTypes.string,
  showPagination: PropTypes.bool,
  headers: PropTypes.arrayOf(PropTypes.shape({
    name: PropTypes.string.isRequired,
    type: PropTypes.oneOf(['text', 'number']),
    key: PropTypes.string.isRequired,
  })).isRequired,
  pageNumber: PropTypes.number,
  totalRecords: PropTypes.number,
  onPageChange: PropTypes.func,
  sortColumn: PropTypes.string,
  sortOrder: PropTypes.string,
  onSortChange: PropTypes.onSortChange,
  data: PropTypes.arrayOf(PropTypes.object).isRequired,
};
