import {ExchangeRate} from './components/exchange-rate';
import {connect} from 'react-redux';
import { apiUrl } from '../src/helpers';
import moment from 'moment'
import axios from 'axios';
import {logException} from "./helpers";
import {getViewExchangeRatePermissions} from "./permissions/ComponentPermissions";
import {Permissioned} from "./permissions/permissions";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => {
  return {
    exchangeRates:state.reducer.currentExchangeRates,
    exchangeRatesError: state.reducer.exchangeRatesError
  }
};

export const mapDispatchToProps = (dispatch) => ({
  onExchangeRatesFetchRequest: async () => {
    try {
      const now = moment.utc().format();
      const response = await axios.get(apiUrl(`exchange-rates/latest?&on=${now}`));
      const exchangeRates = response.data;
      logInfo('EXCHANGE_RATES_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'EXCHANGE_RATES_FETCH_SUCCEEDED', exchangeRates: exchangeRates});
    } catch (error) {
      logException('EXCHANGE_RATES_FETCH_FAILED \n' + error);
      let errorMessage = error.message;
      if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin.";
        dispatch({type: 'EXCHANGE_RATES_FETCH_FAILED', exchangeRatesError: errorMessage});
      }
      else {
        dispatch({type: 'EXCHANGE_RATES_FETCH_FAILED',
        exchangeRatesError: (error.response && error.response.data && error.response.data.message) || errorMessage
        });
      }
    }
  },
  onExchangeRatesDestroy : ()=>{
    dispatch({type:'EXCHANGE_RATES_DESTROYED'});
  }
});

const component = connect(mapStateToProps, mapDispatchToProps)(ExchangeRate);
export const ExchangeRateConnector = Permissioned(component,getViewExchangeRatePermissions, false,null );
