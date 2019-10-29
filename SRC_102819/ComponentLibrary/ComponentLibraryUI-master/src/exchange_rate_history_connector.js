import {apiUrl} from '../src/helpers';
import {ExchangeRateHistory} from './components/exchange-rate-history';
import {connect} from 'react-redux';
import axios from 'axios';
import {browserHistory} from 'react-router';
import querystring from 'querystring';
import {logException} from "./helpers";
import {bindActionCreators} from "redux";
import {fetchMasterDataByNameIfNeeded} from "./actions";
import {Permissioned} from "./permissions/permissions";
import {
  getViewExchangeRateHistoryPermissions,
} from "./permissions/ComponentPermissions";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => ({
  exchangeRateHistory: state.reducer.exchangeRateHistory,
  error: state.reducer.error,
  addExchangeRateError: state.reducer.addExchangeRateError,
  exchangeRateAdding: state.reducer.exchangeRateAdding || false,
  pageNumber: props.location.query.pageNumber,
  sortOrder: props.location.query.sortOrder,
  sortColumn: props.location.query.sortColumn,
  currencyType: props.location.query.currencyType,
  appliedFrom: props.location.query.appliedFrom,
  currencyData: state.reducer.masterDataByName['currency']
});

export const mapDispatchToProps = (dispatch, ownProps) => ({
  onExchangeRateHistoryFetchRequest: async (pageNumber, sortColumn, sortOrder, currencyType, appliedFrom) => {
    try {
      const response = await axios.get(apiUrl(`/exchange-rates/all`),{params: {pageNumber, sortColumn, sortOrder, currencyType, appliedFrom}});
      const exchangeRateHistory = response.data;
      logInfo('EXCHANGE_RATE_HISTORY_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'EXCHANGE_RATE_HISTORY_FETCH_SUCCEEDED', details: exchangeRateHistory});
    } catch (error) {
      logException('EXCHANGE_RATE_HISTORY_FETCH_FAILED \n' + error);
      let errorMessage = error.message;
      if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin.";
        dispatch({type: 'EXCHANGE_RATE_HISTORY_FETCH_FAILED', error: errorMessage});
      }
      else {
        dispatch({type: 'EXCHANGE_RATE_HISTORY_FETCH_FAILED',
            error: (error.response && error.response.data && error.response.data.message) || errorMessage
        });
      }
    }
  },
  onAddExchangeRate: async (exchangeRate) => {
    try {
      dispatch({type: 'ADD_EXCHANGE_RATE_REQUESTED'});
      let url = apiUrl(`/exchange-rates`);
      await axios.post(url, exchangeRate);
      logInfo('ADD_EXCHANGE_RATE_SUCCEEDED \n' + exchangeRate);
      dispatch({type: 'ADD_EXCHANGE_RATE_SUCCEEDED'});
      window.location.reload();
    } catch (error) {
      logException('ADD_EXCHANGE_RATE_FAILED \n' + error);
      let errorMsg = undefined;
      let errorMessage = error.message;
      if (error.response.status === 409) {
        const FormattedDate = new Date(exchangeRate.appliedFrom);
        const FormattedDateString = FormattedDate.getDate() + "/" + (FormattedDate.getMonth() + 1).toString() + "/" + FormattedDate.getFullYear();
        errorMsg = `Duplicate entries are not allowed. A exchange rate version for '${exchangeRate.fromCurrency}' on '${FormattedDateString}' already exists`;
        dispatch({type: 'ADD_EXCHANGE_RATE_FAILED', error: errorMsg});
      }
      else if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin.";
        dispatch({type: 'ADD_EXCHANGE_RATE_FAILED', error: errorMessage});
      }
      else {
        dispatch({type: 'ADD_EXCHANGE_RATE_FAILED',
        error: (error.response && error.response.data && error.response.data.message) || errorMessage
        });
      }
    }
  },
  onAddExchangeRateErrorClose: () => {
    dispatch({type: 'ADD_EXCHANGE_RATE_ERROR_CLOSED'});
  },
  onDestroyExchangeRateHistory: () => {
    dispatch({type: 'DESTROY_EXCHANGE_RATE_HISTORY'})
  },
  onAmendQueryParams: (params) => {
    const newParams = Object.assign({}, ownProps.location.query, params);
    browserHistory.push(`/exchange-rates-history?${querystring.stringify(newParams)}`);
    dispatch({type: 'EXCHANGE_RATE_HISTORY_DESTROYED'});
  },
  ...bindActionCreators({fetchMasterDataByNameIfNeeded}, dispatch)
});

const component = connect(mapStateToProps, mapDispatchToProps)(ExchangeRateHistory);
export const ExchangeRateHistoryConnector = Permissioned(component, getViewExchangeRateHistoryPermissions, true,null);
