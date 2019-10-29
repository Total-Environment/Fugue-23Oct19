import {EditRates} from "./components/edit-rates/index";
import {connect} from 'react-redux';
import {apiUrl, logException} from "./helpers";
import moment from 'moment';
import axios from 'axios';
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state,props) => {
  return {
    rates: state.reducer.rates.material,
  }
};

export const mapDispatchToProps = (dispatch,props) => ({
  onRateFetchRequest: async () => {
    try {
      const now = moment.utc().format();
      const response = await axios.post(apiUrl('materials/rates'),{appliedOn: now});
      const rates = response.data;
      logInfo('RATES_FETCH_SUCCEEDED \n' + response);
      dispatch({type: 'RATES_FETCH_SUCCEEDED', rates: rates, componentType: 'material'});
    }
    catch (error) {
      logException('RATES_FETCH_FAILED \n '+error);
      let errorMessage = error.message;
      if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin.";
        dispatch({type: 'RATES_FETCH_FAILED', ratesError: errorMessage, componentType: 'material'});
      }
      else {
        dispatch({type: 'RATES_FETCH_FAILED',
          ratesError: (error.response && error.response.data && error.response.data.message) || errorMessage,
          componentType: 'material'
        });
      }
    }
  },

  onDestroyRates: () => {
    dispatch({type: 'DESTROY_RATES', componentType: "material"});
  },
});

export const EditRateConnector = connect(mapStateToProps, mapDispatchToProps)(EditRates);
