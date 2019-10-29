import React, {PropTypes} from 'react';
import {Icon} from '../icon';
import styles from './index.css';
import classNames from 'classnames';
import {idFor} from '../../helpers';

export class SortComponent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {expanded: false, selectedColumn:this.getSelectedColumn()};
    this.handleClick = this.handleClick.bind(this);
  }

  getSelectedColumn(){
    var sortedColumn = this.props.sortableProperties.find((p)=>p.key === this.props.sortColumn);
    return (sortedColumn == null || sortedColumn == undefined) ? '' : sortedColumn.value;
  }

  handleClick(e) {
    e.preventDefault();
    this.setState({expanded: !this.state.expanded});
  }

  toggleSortOrder() {
    return this.props.sortOrder === 'Ascending' ? 'Descending' : 'Ascending';
  }

  handleSort(p) {
    this.props.onSort(p.key, p.key === this.props.sortColumn ? this.toggleSortOrder() : this.props.sortOrder || 'Descending');
    this.setState({expanded: false});
  }

  getClassName(p) {
    if (p !== this.props.sortColumn) return styles.column;
    return classNames({
      [styles.column]: true,
      [styles.selected]: true,
      [styles.ascending]: this.props.sortOrder === 'Ascending',
      [styles.descending]: this.props.sortOrder === 'Descending',
    })
  }

  getSortButtonClassName() {
    return classNames({
      [styles.sortButton]: true,
      [styles.sortButtonExpanded]: this.state.expanded,
      [styles.ascending]: this.props.sortOrder === 'Ascending',
      [styles.descending]: this.props.sortOrder === 'Descending',
    })
  }

  render() {
    return <div className={styles.sort}>
      <button id={idFor('sort button')} className={this.getSortButtonClassName()}
              onClick={this.handleClick}>
        {this.state.selectedColumn}
        <Icon name="sort" className={styles.sortIcon}/>
      </button>
      {this.state.expanded ?
        <div className={styles.expanded}>
          <h2 className={styles.title}>Sort</h2>
          <ul className={styles.list}>
            {this.props.sortableProperties.map(p => <li>
              <button id={idFor('sort '+p.value)} className={this.getClassName(p.key)} onClick={() => this.handleSort(p)}>{p.value}</button>
            </li>)}
          </ul>
        </div> : ''}
    </div>;
  }
}
SortComponent.propTypes = {
  sortableProperties: PropTypes.array.isRequired,
  onSort: PropTypes.func.isRequired,
  sortColumn: PropTypes.string.isRequired,
  sortOrder: PropTypes.oneOf(['Ascending', 'Descending']).isRequired,
};
