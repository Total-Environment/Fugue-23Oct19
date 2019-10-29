import React, { PropTypes } from 'react';
import { Loading } from '../loading';
import { PageTab } from '../page-tab';
import { Search } from '../search';
import styles from './index.css';
import { SortComponent } from '../sort-component';
import { dropKeys } from '../../helpers';
import Pagination from 'rc-pagination';
import locale from 'rc-pagination/lib/locale/en_US';
import { idFor } from '../../helpers';
import { floatingAction } from '../../css-common/forms.css';
import { Link } from 'react-router';
import { FilterComponentConnector } from '../../filter_component_connector';
import classNames from 'classnames';
import { Icon } from "../icon/index";
import { icon } from './index.css';
import * as R from 'ramda';
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";
import {getCreateComponentPermissions} from "../../permissions/ComponentPermissions";

export class ComponentSearch extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      selectedHeader: "general", filter: { shown: false }, isFilterApplied: false, error: {
        message: '',
        shown: false
      }, modifiedDefinition: null, columns: [], levels: null
    };
    this.handleSort = this.handleSort.bind(this);
    this.handleTabChange = this.handleTabChange.bind(this);
    this.renderFilterComponent = this.renderFilterComponent.bind(this);
    this.handleClose = this.handleClose.bind(this);
    this.handleApplyFilters = this.handleApplyFilters.bind(this);
    this.handleClearFilters = this.handleClearFilters.bind(this);
    this.cancelErrorDialog = this.cancelErrorDialog.bind(this);
  }

  title() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        return 'Materials';
      case 'service':
        return 'Services';
      case 'SFG':
        return 'Semi Finished Goods';
      case 'package':
        return 'Packages';
    }
  }

  componentDidMount() {
    if ((this.isFetching(this.props)) || (this.props.error && this.props.error.type) || (this.props.isFetching))
      return;
    this.fetchResults(this.props);
  }

  fetchResults(props) {
    switch (props.componentType.toLowerCase()) {
      case 'material':
        props.onMaterialSearchResultsFetch(props.group, props.keyword, props.pageNumber,
          props.isListing, props.sortColumn, props.sortOrder, props.group !== this.props.group ? [] : props.filterData);
        break;
      case 'service':
        props.onServiceSearchResultsFetch(props.group, props.keyword, props.pageNumber,
          props.isListing, props.sortColumn, props.sortOrder, props.filterData);
        break;
      case 'sfg':
        props.onSfgSearchResultsFetch(props.keyword, props.pageNumber,
          props.isListing, props.sortColumn, props.sortOrder, props.filterData);
        break;
      case 'package':
        props.onPackageSearchResultsFetch(props.keyword, props.pageNumber,
          props.isListing, props.sortColumn, props.sortOrder, props.filterData);
        break;
    }
  }

  getSearchItems() {
    return this.props.search && this.props.search.values && this.props.search.values.items;
  }

  areResultsAvailable(props) {
    //search is defined and No Errors
    return !!(props.search && props.search.values);
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.isFiltering && nextProps.pageNumber === 1
      && (this.props.sortColumn === nextProps.sortColumn) && (this.props.sortOrder === nextProps.sortOrder)
      && this.state.columns.length === 0) {
      return
    }
    if (this.shouldFetch(nextProps) || this.propsHaveChanged(nextProps)) {
      this.fetchResults(nextProps);
    }
    if(nextProps.group !== this.props.group) {
      let level1 = this.areResultsAvailable(nextProps) && !this.isFetching(nextProps) && nextProps.search && nextProps.search.values.items[0].headers.find(h => h.key === "classification").columns[0].value;
      let level2 = this.areResultsAvailable(nextProps) && !this.isFetching(nextProps) && nextProps.search && nextProps.search.values.items[0].headers.find(h => h.key === "classification").columns[1].value;
      if((nextProps.componentType === 'material' && (level2 !== false && level2 !== nextProps.group)) ||
        (nextProps.componentType === 'service' && (level1 !== false && level1 !== nextProps.group))) {
        return;
      }
      if(level1 !== false) {
        this.setState({levels: [level1,level2], isFilterApplied: false});
      }
    }
  }

  isFetching(props) {
    return !!(props && props.search && props.search.isFetching);
  }

  shouldFetch(props) {
    return !this.isFetching(props) && !this.areResultsAvailable(props) && !props.error;
  }

  propsHaveChanged(nextProps) {
    return !(nextProps.group === this.props.group
      && nextProps.keyword === this.props.keyword
      && nextProps.sortOrder === this.props.sortOrder
      && nextProps.sortColumn === this.props.sortColumn
      && nextProps.pageNumber === this.props.pageNumber
    );
  }

  componentWillUnmount() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.onMaterialSearchDestroy();
        break;
      case 'service':
        this.props.onServiceSearchDestroy();
        break;
      case 'sfg':
        this.props.onSfgSearchDestroy();
        break;
      case 'package':
        this.props.onPackageSearchDestroy();
        break;
    }
  }

  handleSort(sortColumn, sortOrder) {
    const params = { pageNumber: 1, sortColumn, sortOrder };
    this.props.onAmendQueryParams(params);
  }

  getComponents() {
    return this.getSearchItems().map(i => dropKeys(i, ['id', 'group']));
  }

  getColumns(components, selectedHeader) {
    let columns = [];
    let header = components && components[0] && components[0].headers.find(header => header.key === selectedHeader);
    header && header.columns.map(column => columns.push({value: column.name, key: column.key}));
    return columns;
  }

  renderSearchResult() {
    const components = this.getComponents();
    const selectedHeader = this.props.selectedTab || 'general';
    return [
      this.props.isListing ? <h4 className={styles.count}>Last {this.getSearchItems().length} Item(s)</h4>
        : this.props.group ?
          <h4 id={idFor('display records')} className={styles.count}>Displaying {this.getSearchItems().length} of {this.props.search.values.recordCount} records found in <span
            className={styles.bold}>{this.title()} > {this.props.group}</span></h4> : <h4 id={idFor('display records')} className={styles.count}>Displaying {this.getSearchItems().length} of {this.props.search.values.recordCount} records found</h4>,
      this.props.isListing ? '' :
        [
          <SortComponent sortOrder={this.props.search.values.sortOrder} sortColumn={this.props.search.values.sortColumn}
            sortableProperties={this.getColumns(components, selectedHeader)} onSort={this.handleSort} />],
      <PageTab
        componentType={this.props.componentType}
        values={components}
        isListing={this.props.isListing}
        onSort={this.handleSort}
        selectedTab={selectedHeader}
        onTabChange={this.handleTabChange}
      />,
      <PermissionedForNonComponents allowedPermissions={getCreateComponentPermissions(this.props)}>
      <Link className={floatingAction} to={`/${this.props.componentType.toLowerCase()}s/new`}>+</Link>
      </PermissionedForNonComponents>,
      this.props.isListing ? '' :
        <Pagination id={idFor('pagination')} current={this.props.pageNumber}
          total={this.props.search.values.recordCount}
          pageSize={this.props.search.values.batchSize} showSizeChanger={false}
          onChange={(pageNumber) => this.props.onAmendQueryParams({ pageNumber })}
          locale={locale}
        />,
    ];
  }

  handleClearFilters() {
    if (this.state.columns.length === 0) {
      this.setState({ filter: { shown: false } });
      return;
    }
    const params = { pageNumber: 1 };
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.onAmendQueryParams(params);
        this.props.onMaterialSearchResultsFetch(this.props.group, this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder);
        break;
      case 'service':
        this.props.onAmendQueryParams(params);
        this.props.onServiceSearchResultsFetch(this.props.group, this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder);
        break;
      case 'sfg':
        this.props.onAmendQueryParams(params);
        this.props.onSfgSearchResultsFetch(this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder);
        break;
      case 'package':
        this.props.onAmendQueryParams(params);
        this.props.onPackageSearchResultsFetch(this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder);
        break;
    }
    this.setState({ filter: { shown: false }, isFilterApplied: false, modifiedDefinition: null, columns: [] });
  }

  handleApplyFilters(filterData) {
    let columns = [];
    filterData.headers.forEach((header) => {
      header.columns.forEach((column) => {
        if (column !== null && column.value !== null && column.value !== '') {
          if (column.dataType.name === "Money") {
            columns.push({
              ColumnKey: column.key, ColumnValue:
              column.value.amount + ' ' + column.value.currency
            });
          }
          else if (column.dataType.name === "Unit") {
            columns.push({
              ColumnKey: column.key, ColumnValue:
              column.value.value
            });
          }
          else {
            columns.push({ ColumnKey: column.key, ColumnValue: column.value });
          }
        }
      });
    });
    columns = R.compose(
      R.filter(c => !R.isEmpty(c.ColumnValue)),
      R.map(c => R.merge(c, { ColumnValue: R.is(String, c.ColumnValue) ? R.trim(c.ColumnValue) :c.ColumnValue }))
    )(columns);

    if (!this.state.columns.length && !columns.length) {
      this.setState({ filter: { shown: false } });
      return;
    }
    if (!this.isFormValid(filterData)) {
      this.setState({ error: { message: 'There are errors in the form. Please fix it and submit.', shown: true } });
      return;
    }
    const params = { pageNumber: 1, filterData: columns };
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.onAmendQueryParams(params);
        this.props.onMaterialSearchResultsFetch(this.props.group, this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder, columns);
        break;
      case 'service':
        this.props.onAmendQueryParams(params);
        this.props.onServiceSearchResultsFetch(this.props.group, this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder, columns);
        break;
      case 'sfg':
        this.props.onAmendQueryParams(params);
        this.props.onSfgSearchResultsFetch(this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder, columns);
        break;
      case 'package':
        this.props.onAmendQueryParams(params);
        this.props.onPackageSearchResultsFetch(this.props.keyword, 1,
          this.props.isListing, this.props.sortColumn, this.props.sortOrder, columns);
        break;
    }
    this.setState({ filter: { shown: false }, isFilterApplied: true, modifiedDefinition: filterData, columns: columns });
  }

  isFormValid(filterData) {
    let isValid = true;
    const newState = JSON.parse(JSON.stringify(filterData));
    newState.headers
      .forEach((header) => {
        header.columns.forEach((column) => {
          if (column !== null
            && column.value !== null && !column.validity.isValid) {
            isValid = false;
          }
        })
      });
    return isValid;
  }

  showErrorDialog(message) {
    this.setState({ error: { message, shown: true } });
  }

  handleClose() {
    this.setState({ filter: { shown: false } });
  }

  handleTabChange(header) {
    const params = { selectedTab: header };
    this.props.onAmendQueryParams(params);
  }

  capitalizeFirstLetter(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }

  renderHeading() {
    const content = this.props.isListing ? (this.props.error ?
      `No ${this.props.componentType}s found in Component Library`
      : `${this.capitalizeFirstLetter(this.props.componentType)} Updates`) :
      (this.props.error ? 'No results found for ' : 'Search results for ');
    return <h2 id={idFor('search result')} className={styles.title}>{content}<span
      className={styles.keyword}>{this.props.keyword}</span></h2>;
  }

  renderWaiting() {
    return this.props.error ? '' : <Loading />;
  }

  render() {
    if(this.props.error && this.props.error.type === "NetworkError") {
      return <h3 id={idFor('error')}>{this.props.error.error}</h3>;
    }
    return <div className={styles.search}>
      {this.renderFilterButton()}
      <Search componentType={this.props.componentType} keyword={this.props.keyword} focused="true" />
      {this.renderHeading()}
      {this.areResultsAvailable(this.props) && !this.isFetching(this.props) ? this.renderSearchResult() : this.renderWaiting()}
    </div>;
  }

  renderFilterComponent() {
    this.setState({ filter: { shown: true } });
  }

  cancelErrorDialog() {
    this.setState({ error: { message: '', shown: false } });
  }

  getLevelInformation() {
    let level1 = this.areResultsAvailable(this.props) && !this.isFetching(this.props) && this.props.search && this.props.search.values.items[0].headers.find(h => h.key === "classification").columns[0].value;
    let level2 = this.areResultsAvailable(this.props) && !this.isFetching(this.props) && this.props.search && this.props.search.values.items[0].headers.find(h => h.key === "classification").columns[1].value;

    if(!this.state.levels && level1 !== false) {
        this.setState({levels: [level1,level2]});
        return [level1, level2];
    }
    if(this.state.levels && ((this.state.levels[0] !== level1 && level1 !== false) || (this.state.levels[1] !== level2 && level2 !== false))) {
      this.setState({levels: [level1,level2]});
      return [level1, level2];
    }
    return this.state.levels;
  }

  renderFilterButton() {
    let levels = this.getLevelInformation();
    const columns = this.state.columns;
    let classes = this.state.isFilterApplied && columns && columns.length !== 0 ? classNames(styles.filtersApplied) : classNames(styles.filter);
    return ([this.props.isListing ? '' : <button id={idFor('filter')} className={classes} onClick={(e) => {
      e.preventDefault();
        this.renderFilterComponent()
    }}>{this.state.isFilterApplied && columns && columns.length !== 0 ? <Icon name="rightMark" className={icon} /> : ''}Filter</button>,
    this.state.filter.shown && <FilterComponentConnector
      onClose={this.handleClose}
      group={this.props.componentType === 'SFG' ? 'sfg' : this.props.componentType === 'package'? 'package':this.props.group}
      componentType={this.props.componentType}
      onApply={this.handleApplyFilters}
      onClear={this.handleClearFilters}
      onCancelErrorDialog={this.cancelErrorDialog}
      errorInFilter={this.state.error}
      modifiedDefinition={this.state.modifiedDefinition}
      levels={levels}
    />]);
  }
}

ComponentSearch.propTypes = {
  isFetching: React.PropTypes.bool.isRequired,
  values: React.PropTypes.shape({
    items: React.PropTypes.array.isRequired,
    recordCount: React.PropTypes.number.isRequired,
    totalPages: React.PropTypes.number.isRequired,
    batchSize: React.PropTypes.number.isRequired,
  }),
  error: React.PropTypes.shape({
    type: React.PropTypes.string.isRequired,
    error: React.PropTypes.object,
  }),
};
