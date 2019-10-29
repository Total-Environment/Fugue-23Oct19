import React from 'react';
import styles, {icon} from "./index.css"
import {Loading} from '../loading';
import {CreateRentalRate} from '../create-rental-rate';
import {Link} from 'react-router';
import {Table} from "../table/index";
import {idFor, isFetched, isFetchedAll} from "../../helpers"
import classNames from "classnames"
import {RateHistoryFilterComponent} from "../rate-history-filter-component"
import {Icon} from "../icon/index"
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";

export class RentalRateHistory extends React.Component {
  constructor(props) {
    super(props);
    this.renderHeader = this.renderHeader.bind(this);
    this.renderDate = this.renderDate.bind(this);
    this.handleClose = this.handleClose.bind(this);
    this.handleApplyFilters = this.handleApplyFilters.bind(this);
    this.handleClearFilters = this.handleClearFilters.bind(this);
    this.state = {filter: {shown: false}, isFilterApplied: false};
  }

  filterDefinition() {
    return {
      filterData: {
        rentalUnit: {
          name: 'Rental Unit',
          value: '',
          id: 'rentalUnit',
          type: 'MasterData',
          data: this.props.rentalUnitData
        }, appliedFrom: {
          name: 'Applied From',
          value: null,
          id: 'appliedFrom',
          type: 'Date',
        }
      }, error: {
        message: "", //this.props.errorInFilter.message,
        shown: false //this.props.errorInFilter.shown
      }
    }
  }

  filteredDefinition() {
    return {
      filterData: {
        rentalUnit: {
          name: 'Rental Unit',
          value: this.props.rentalUnit || '',
          id: 'rentalUnit',
          type: 'MasterData',
          data: this.props.rentalUnitData
        }, appliedFrom: {
          name: 'Applied From',
          value: this.props.appliedFrom || null,
          id: 'appliedFrom',
          type: 'Date',
        }
      }, error: {
        message: "", //this.props.errorInFilter.message,
        shown: false //this.props.errorInFilter.shown
      }
    }
  }

  componentDidMount() {
    window.scrollTo(0,0);
    this.updateData(this.props);
  }

  componentWillReceiveProps(nextProps) {
    this.updateData(nextProps);
  }

  updateData(nextProps) {
    if (!nextProps.rentalRateHistory.values && !nextProps.rentalRateHistory.isFetching && !nextProps.rentalRateHistory.error) {
      const materialCode = nextProps.params.materialCode;
      const {pageNumber, sortColumn, sortOrder, rentalUnit, appliedFrom} = nextProps.location.query;
      nextProps.onRentalRateHistoryFetchRequest(materialCode, pageNumber, sortColumn, sortOrder, rentalUnit, appliedFrom);
      nextProps.fetchMasterDataByNameIfNeeded('rental_unit');
      nextProps.fetchMasterDataByNameIfNeeded('currency');
    }
  }

  render() {
    return (<div className={styles.rateHistory}>
      <h2 className={styles.title}>
        {this.props.materialCode}
        {this.renderFilterButton()}
      </h2>
      <Link className={styles.backToComponent} to={`/materials/${this.props.materialCode}`}>Back to material
        details</Link>
      {isFetchedAll(this.props.rentalUnitData, this.props.currencyData) &&
      <CreateRentalRate addRentalRateError={this.props.addRentalRateError}
                        rentalRateAdding={this.props.rentalRateAdding}
                        onAddRentalRate={this.props.onAddRentalRate}
                        componentCode={this.props.materialCode}
                        onAddRentalRateErrorClose={this.props.onAddRentalRateErrorClose}
                        rentalUnitData={this.props.rentalUnitData}
                        currencyData={this.props.currencyData}
      />}
      {!!this.props.rentalRateHistory.values ? this.renderRentalRateHistory() : (!!this.props.rentalRateHistory.error ? this.renderNoRentalRateHistory() :
        <Loading />)}
    </div>);
  }

  renderFilterButton() {
    let classes = (this.state.isFilterApplied || this.props.rentalUnit || this.props.appliedFrom) ? classNames(styles.filtersApplied) : classNames(styles.filter);
    return (
      [<button id={idFor('filter')} className={classes} onClick={ (event) => {
        event.preventDefault();
        this.renderFilterComponent();
      }}>{(this.state.isFilterApplied || this.props.rentalUnit || this.props.appliedFrom) ? <Icon name="rightMark" className={icon}/> : ''}Filter</button>,
        isFetchedAll(this.props.rentalUnitData, this.props.currencyData) &&
        <RateHistoryFilterComponent
          componentType={this.props.componentType}
          definition={this.filterDefinition()}
          changedDefinition={this.filteredDefinition()}
          isOpen={this.state.filter.shown}
          onClose={this.handleClose}
          onApply={this.handleApplyFilters}
          onClear={this.handleClearFilters}/>]
    )
  }

  renderFilterComponent() {
    this.setState({filter: {shown: true}});
  }

  handleClose() {
    this.setState({filter: {shown: false}});
  }

  handleApplyFilters(filterData) {
    let filters = {};
    for (let key in filterData) {
      if (filterData[key].value !== null)
        filters[key] = filterData[key].value;
    }

    if (!Object.keys(filters).length) {
      this.setState({filter: {shown: false}});
      return;
    }
    // if (!this.isFormValid(filterData)) {
    //   this.setState({error: {message:'There are errors in the form. Please fix it and submit.', shown: true}});
    //   return;
    // }
    const params = {pageNumber: 1, ...filters};
    this.props.onAmendQueryParams(params);
    this.setState({filter: {shown: false}, isFilterApplied: true});
  }

  handleClearFilters(filterData) {
    let filters = {};
    for (let key in filterData) {
      filters[key] = filterData[key].value;
    }
    const params = {pageNumber: 1, ...filters};
    this.props.onAmendQueryParams(params);
    this.setState({filter: {shown: false}, isFilterApplied: false});
  }

  renderRentalRateHistory() {
    const data = this.props.rentalRateHistory.values.items.map(rate => this.renderRentalRate(rate));
    return <Table
      headers={[
        {name: 'Rental Unit', key: 'Rental Unit', sortKey: 'UnitOfMeasure', sortable: true},
        {name: 'Rental Rate', key: 'Rental Rate', type: 'number', sortable: false},
        {name: 'Applied From', key: 'Applied From', type: 'number', sortKey: 'AppliedFrom', sortable: true}]}
      data={data}
      showPagination
      pageNumber={this.props.rentalRateHistory.values.pageNumber}
      totalRecords={this.props.rentalRateHistory.values.totalRecords}
      batchSize={this.props.rentalRateHistory.values.batchSize}
      sortColumn={this.props.rentalRateHistory.values.sortColumn}
      sortOrder={this.props.rentalRateHistory.values.sortOrder}
      onSortChange={(sortColumn, sortOrder) => this.props.onAmendQueryParams({sortColumn, sortOrder, pageNumber: 1})}
      onPageChange={(pageNumber) => this.props.onAmendQueryParams({pageNumber})}
    />;
  }

  renderNoRentalRateHistory() {
    return <h2 className={styles.errorMessage}>{this.props.rentalRateHistory.error}</h2>;
  }

  renderHeader() {
    return (
      <tr className={styles.tableHeader}>
        <th className={styles.text}>Rental Unit</th>
        <th className={styles.number}>Rental Rate</th>
        <th className={styles.number}>Applied From</th>
      </tr>);
  }

  renderCurrency(value) {
    return <span className={styles.currencyContainer}>
          <span className={styles.moneyValue}>{value.value}</span>
          <span className={styles.moneyCurrency}>{value.currency}</span>
        </span>;
  }

  renderRentalRate(rentalRate) {
    return {
      'Rental Unit': rentalRate.unitOfMeasure,
      'Rental Rate': rentalRate.rentalRateValue ? this.renderCurrency(rentalRate.rentalRateValue) : '-',
      'Applied From': this.renderDate(rentalRate.appliedFrom),
    }
  }

  renderDate(date) {
    const formattedDate = new Date(date);
    return `${formattedDate.getDate()}/${(formattedDate.getMonth() + 1).toString()}/${formattedDate.getFullYear()}`;
  }
}
