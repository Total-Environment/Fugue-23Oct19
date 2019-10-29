import React from "react"
import styles, {icon} from "./index.css"
import {Loading} from "../loading"
import {CreateRate} from "../create-rate"
import {Link} from "react-router"
import {Table} from "../table"
import {idFor, isFetched, isFetchedAll} from "../../helpers"
import classNames from "classnames"
import {RateHistoryFilterComponent} from "../rate-history-filter-component"
import {Icon} from "../icon/index"

export class RateHistory extends React.Component {
  constructor(props) {
    super(props);
    this.getHeader = this.getHeader.bind(this);
    this.renderDate = this.renderDate.bind(this);
    this.handleClose = this.handleClose.bind(this);
    this.handleApplyFilters = this.handleApplyFilters.bind(this);
    this.handleClearFilters = this.handleClearFilters.bind(this);
    this.state = {filter: {shown: false}, isFilterApplied: props.typeOfPurchase || props.appliedOn || props.location || false};
  }

  filterDefinition() {
    return {
      filterData: {
        typeOfPurchase: {
          name: 'Type of Purchase',
          value: '',
          id: 'typeOfPurchase',
          type: 'MasterData',
          data: this.props.typeOfPurchaseData
        }, location: {
          name: 'Location',
          value: '',
          id: 'location',
          type: 'MasterData',
          data: this.props.locationData
        }, appliedOn: {
          name: 'Applied On',
          value: null,
          id: 'appliedOn',
          type: 'Date'
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
        typeOfPurchase: {
          name: 'Type of Purchase',
          value: this.props.typeOfPurchase || '',
          id: 'typeOfPurchase',
          type: 'MasterData',
          data: this.props.typeOfPurchaseData
        }, location: {
          name: 'Location',
          value: this.props.location || '',
          id: 'location',
          type: 'MasterData',
          data: this.props.locationData
        }, appliedOn: {
          name: 'Applied On',
          value: this.props.appliedOn || null,
          id: 'appliedOn',
          type: 'Date'
        }
      }, error: {
        message: "", //this.props.errorInFilter.message,
        shown: false //this.props.errorInFilter.shown
      }
    }
  }

  componentDidMount() {
    window.scrollTo(0, 0);
    this.fetchData(this.props);
  }

  componentWillReceiveProps(nextProps) {
    this.fetchData(nextProps);
  }

  fetchData(props) {
    if (props.rateHistory && props.rateHistory.items) {
      return;
    }
    if(!props.error) {
      props.onRateHistoryFetchRequest(props.componentCode, props.componentType, props.pageNumber, props.sortColumn, props.sortOrder, props.typeOfPurchase, props.location, props.appliedOn);
    }
    if(!props.locationError) {
      props.fetchMasterDataByNameIfNeeded('location');
    }
    if(!props.typeOfPurchaseError) {
      props.fetchMasterDataByNameIfNeeded('type_of_purchase');
    }
    if(!props.currencyError) {
      props.fetchMasterDataByNameIfNeeded('currency');
    }
  }

  componentWillUnmount() {
    this.props.onDestroyRateHistory();
  }

  render() {
    return (<div className={styles.rateHistory}>
      <h2 className={styles.title}>
        {this.props.componentCode}
        {!this.props.error &&
        !this.props.typeOfPurchaseError &&
        this.props.currencyError &&
        this.props.locationError &&
        this.renderFilterButton()}
      </h2>
      <Link className={styles.backToComponent} to={`/${this.props.componentType}s/${this.props.componentCode}`}>Back
        to {this.props.componentType} details</Link>
      {isFetched(this.props.typeOfPurchaseData) && isFetched(this.props.locationData) && isFetched(this.props.currencyData) &&
      <CreateRate addRateError={this.props.addRateError}
                  componentType={this.props.componentType}
                  onAddRateErrorClose={this.props.onAddRateErrorClose}
                  onAddRate={this.props.onAddRate}
                  componentCode={this.props.componentCode}
                  rateAdding={this.props.rateAdding}
                  locationData={this.props.locationData}
                  typeOfPurchaseData={this.props.typeOfPurchaseData}
                  currencyData={this.props.currencyData}
      />}
      {this.props.rateHistory && this.props.rateHistory.items && this.props.rateHistory.items.length ? this.renderRateHistory() : ((this.props.error || this.props.typeOfPurchaseError || this.props.locationError || this.props.currencyError) ? this.renderNoRateHistory() :
        <Loading />)}
    </div>);

  }

  renderFilterButton() {
    let classes = (this.state.isFilterApplied || this.props.typeOfPurchase || this.props.appliedOn || this.props.location) ? classNames(styles.filtersApplied) : classNames(styles.filter);
    return (
      [<button id={idFor('filter')} className={classes} onClick={ (event) => {
        event.preventDefault();
        this.renderFilterComponent();
      }}>{(this.state.isFilterApplied || this.props.typeOfPurchase || this.props.appliedOn || this.props.location) ? <Icon name="rightMark" className={icon}/> : ''}Filter</button>,
        isFetchedAll(this.props.currencyData, this.props.typeOfPurchaseData, this.props.locationData) &&
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

  renderNoRateHistory() {
    return <h3 className={styles.errorMessage}>{this.props.error || this.props.typeOfPurchaseError || this.props.locationError || this.props.currencyError}</h3>;
  }

  renderRateHistory() {
    const {items, pageNumber, batchSize, totalRecords, sortColumn, sortOrder} = this.props.rateHistory;
    return <Table className={styles.table}
                  headers={this.getHeader()}
                  data={items.map(rate => this.getRate(rate))}
                  showPagination
                  pageNumber={pageNumber}
                  batchSize={batchSize}
                  totalRecords={totalRecords}
                  sortColumn={sortColumn}
                  sortOrder={sortOrder}
                  onPageChange={(pageNumber) => this.props.onAmendQueryParams({pageNumber})}
                  onSortChange={(sortColumn, sortOrder) => this.props.onAmendQueryParams({
                    sortColumn,
                    sortOrder
                  })}
    />;
  }

  getHeader() {
    return [
      {name: 'Location', key: 'location', sortKey: 'Location', type: 'text', sortable: true},
      {name: 'Type of Purchase', key: 'typeOfPurchase', sortKey: 'TypeOfPurchase', type: 'text', sortable: true},
      {name: 'Landed Rate', key: 'landedRate', sortKey: 'LandedRate.Value', type: 'number', sortable: false},
      {
        name: 'Control Base Rate',
        key: 'controlBaseRate',
        sortKey: 'ControlBaseRate.Value',
        type: 'number',
        sortable: false
      },
      ...this.getCoefficientsForComponent(this.props.componentType),
      {name: 'Applied From', key: 'appliedOn', sortKey: 'AppliedOn', type: 'number', sortable: true},
    ];
  }

  getCurrency(value) {
    return value ? <span className={styles.currencyContainer}>
                <span className={styles.moneyValue}>{value.value}</span>
                <span className={styles.moneyCurrency}>{value.currency}</span>
            </span> : '-';
  }

  getCoefficientsForComponent(componentType) {
    switch(componentType) {
      case 'material' :
        return this.getMaterialCoefficients();
        break;
      case 'service' :
        return this.getServiceCoefficients();
    }
  }

  getMaterialCoefficients() {
    return [
      {name: 'Insurance Charges (%)', key: 'insuranceCharges', sortKey: 'insuranceCharges', type: 'number', sortable: false},
      {name: 'Freight Charges (%)', key: 'freightCharges', sortKey: 'freightCharges', type: 'number', sortable: false},
      {name: 'Basic Customs Duty (%)', key: 'basicCustomsDuty', sortKey: 'basicCustomsDuty', type: 'number', sortable: false},
      {name: 'Clearance Charges (%)', key: 'clearanceCharges', sortKey: 'clearanceCharges', type: 'number', sortable: false},
      {name: 'Tax Variance (%)', key: 'taxVariance', sortKey: 'taxVariance', type: 'number', sortable: false},
      {name: 'Location Variance (%)', key: 'locationVariance', sortKey: 'locationVariance',type: 'number', sortable: false},
      {name: 'Market Fluctuation (%)', key: 'marketFluctuation', sortKey: 'marketFluctuation',type: 'number', sortable: false}
    ];
  }

  getServiceCoefficients() {
    return [
      {name: 'Location Variance (%)', key: 'locationVariance', sortKey: 'locationVariance', type: 'number', sortable: false},
      {name: 'Market Fluctuation (%)', key: 'marketFluctuation', sortKey: 'marketFluctuation', type: 'number', sortable: false}
    ]
  }

  getRate(rate) {
    return Object.assign({}, {
      location: rate.location,
      typeOfPurchase: rate.typeOfPurchase,
      landedRate: this.getCurrency(rate.landedRate),
      controlBaseRate: this.getCurrency(rate.controlBaseRate),
      appliedOn: this.renderDate(rate.appliedOn),
      insuranceCharges: rate.insuranceCharges,
      freightCharges: rate.freightCharges,
      basicCustomsDuty: rate.basicCustomsDuty,
      clearanceCharges: rate.clearanceCharges,
      taxVariance: rate.taxVariance,
      locationVariance: rate.locationVariance,
      marketFluctuation: rate.marketFluctuation
    });
  }

  renderDate(date) {
    const FormattedDate = new Date(date);
    return `${FormattedDate.getDate()}/${(FormattedDate.getMonth() + 1).toString()}/${FormattedDate.getFullYear()}`;
  }
}

