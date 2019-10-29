import {apiUrl} from '../src/helpers';
import {RateHistory} from './components/rate-history/index';
import {connect} from 'react-redux';
import axios from 'axios';
import {browserHistory} from 'react-router';
import querystring from 'querystring';
import {logException} from "./helpers";
import {bindActionCreators} from "redux";
import {fetchMasterDataByNameIfNeeded} from "./actions";
import {Permissioned, permissions} from "./permissions/permissions";
import {getViewComponentRateHistoryPermissions} from "./permissions/ComponentPermissions";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => ({
  componentCode: props.params.componentCode,
  componentType: props.route.componentType,
  rateHistory: state.reducer.rateHistory,
  error: state.reducer.rateHistoryError,
  addRateError: state.reducer.addRateError,
  rateAdding: state.reducer.rateAdding || false,
  pageNumber: props.location.query.pageNumber,
  sortOrder: props.location.query.sortOrder,
  sortColumn: props.location.query.sortColumn,
  typeOfPurchase: props.location.query.typeOfPurchase,
  location: props.location.query.location,
  appliedOn: props.location.query.appliedOn,
  typeOfPurchaseData: state.reducer.masterDataByName['type_of_purchase'],
  locationData: state.reducer.masterDataByName['location'],
  currencyData: state.reducer.masterDataByName['currency'],
  typeOfPurchaseError: state.reducer.masterDataByName['type_of_purchase'] && state.reducer.masterDataByName['type_of_purchase'].error,
  locationError: state.reducer.masterDataByName['location'] && state.reducer.masterDataByName['location'].error,
  currencyError: state.reducer.masterDataByName['currency'] && state.reducer.masterDataByName['currency'].error,
});

export const mapDispatchToProps = (dispatch, ownProps) => ({
  onRateHistoryFetchRequest: async (componentCode, componentType, pageNumber, sortColumn, sortOrder, typeOfPurchase, location, appliedOn) => {
    try {
      const url = {
        'material': `material-rates/all`,
        'service': `service-rates/all`
      };
      const params = {
        [`${componentType}Id`]: componentCode,
        pageNumber,
        sortColumn,
        sortOrder,
        typeOfPurchase,
        location,
        appliedOn
      };
      const response = await axios.get(apiUrl(url[componentType]), {params});
      const rateHistory = response.data;
      dispatch({type: 'RATE_HISTORY_FETCH_SUCCEEDED', details: rateHistory});
    }
    catch (error) {
      logException('RATE_HISTORY_FETCH_FAILED \n'+error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'RATE_HISTORY_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'RATE_HISTORY_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'RATE_HISTORY_FETCH_FAILED', error: 'No rate history is found'});
      }
      else if(error.response && error.response.status === 400) {
        dispatch({type: 'RATE_HISTORY_FETCH_FAILED', error: ( error.response && error.response.data && error.response.data.message ) || error.message});
      }
      else {
        let errorMessage = error.message;
        if (error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin.";
          dispatch({type: 'RATE_HISTORY_FETCH_FAILED', error: errorMessage});
        }
        else {
          dispatch({
            type: 'RATE_HISTORY_FETCH_FAILED',
            error: ( error.response && error.response.data && error.response.data.message ) || error.message
          });
        }
      }
    }
  },
  onAddRate: async (rate, componentType) => {
    try {
      dispatch({type: 'ADD_RATE_REQUESTED'});
      const urlPart = {
        'material': `/material-rates`,
        'service': `/service-rates`
      };
      await axios.post(apiUrl(urlPart[componentType]), rate);
      logInfo('ADD_RATE_SUCCEEDED \n' + rate);
      dispatch({type: 'ADD_RATE_SUCCEEDED'});
      window.location.reload();
    } catch (error) {
      let errorMsg = undefined;
      if (error.response.status == 409) {
        const FormattedDate = new Date(rate.appliedOn);
        const FormattedDateString = FormattedDate.getDate() + "/" + (FormattedDate.getMonth() + 1).toString() + "/" + FormattedDate.getFullYear();
        errorMsg = `Duplicate entries are not allowed. A rate version on '${FormattedDateString}' for '${rate.location}' with mode of purchase as '${rate.typeOfPurchase}' already exists`;
        logException(errorMsg + '\n' + error);
      }
      else if(error.response && error.response.status === 400) {
        logException('ADD_RATE_SUCCEEDED \n No exchange rate is found for currency' + error);
        if(error.response.data && error.response.data.message.indexOf('No exchange rate is found for currency') !== -1) {
          dispatch({type: 'ADD_RATE_SUCCEEDED'});
          window.location.reload();
          return;
        }
        else {
          logException('RATE_HISTORY_FETCH_FAILED \n' +error);
          dispatch({type: 'RATE_HISTORY_FETCH_FAILED', error: error.response.data.message || error.message});
          return;
        }
      }
      if (!errorMsg) {
        errorMsg = error.response ? error.response.data.message : error.message;
      }
      dispatch({type: 'ADD_RATE_FAILED', error: errorMsg});
    }
  },
  onAddRateErrorClose: () => {
    dispatch({type: 'ADD_RATE_ERROR_CLOSED'});
  },
  onDestroyRateHistory: () => {
    dispatch({type: 'DESTROY_RATE_HISTORY'});
  },
  onAmendQueryParams: (params) => {
    const newParams = Object.assign({}, ownProps.location.query, params);
    browserHistory.push(`/${ownProps.route.componentType}s/${ownProps.params.componentCode}/rate-history?${querystring.stringify(newParams)}`);
    dispatch({type: 'DESTROY_RATE_HISTORY'});
  },
  ...bindActionCreators({fetchMasterDataByNameIfNeeded}, dispatch)
});
const component  = connect(mapStateToProps, mapDispatchToProps)(RateHistory);

export const RateHistoryConnector = Permissioned(component, getViewComponentRateHistoryPermissions, true, null);
