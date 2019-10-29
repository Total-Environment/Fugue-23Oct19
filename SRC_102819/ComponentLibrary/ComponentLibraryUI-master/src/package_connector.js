import {Package} from './components/package';
import {connect} from 'react-redux';
import {apiUrl} from '../src/helpers';
import {onPackageFetchRequest} from "./actions";
import moment from 'moment-timezone';
import axios from 'axios';
import {Permissioned, permissions} from "./permissions/permissions";
import {getComponentViewPermissions, getViewPackagePermissions} from "./permissions/ComponentPermissions";
import {logException} from "./helpers";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => {
	return {
		packageCode: props.params.packageCode,
		details: state.reducer.components[props.params.packageCode],
		error: state.reducer.error,
		cost: state.reducer.packageCosts[props.params.packageCode],
		packageCostError: state.reducer.packageCostError
	}
};

export const mapDispatchToProps = (dispatch, ownProps) => ({
	onPackageFetchRequest: code => onPackageFetchRequest(code)(dispatch),
	onPackageDestroy: () => {
		dispatch({type: 'PACKAGE_DESTROY'});
	},
	onPackageCostFetchRequest: async (location = "Bangalore", date = moment.utc().format()) => {
		try {
			const response = await axios.get(apiUrl(`packages/${ownProps.params.packageCode}/cost`) + `?location=${location}&appliedOn=${date}`);
			const data = response.data;
      logInfo('PACKAGE_COST_FETCH_SUCCEEDED \n' + response.data);
			dispatch({type: 'PACKAGE_COST_FETCH_SUCCEEDED', cost: data, packageCode: ownProps.params.packageCode});
		}
		catch (error) {
		  logException('PACKAGE_COST_FETCH_FAILED \n' + error);
			if(error.response && error.response.status === 404) {
				dispatch({type: 'PACKAGE_COST_FETCH_FAILED', error: error.response && error.response.data});
			}
			else if(error.response && error.response.status === 400) {
				dispatch({type: 'PACKAGE_COST_FETCH_FAILED', error: error.response.data.message});
			}
			else {
				dispatch({
					type: 'PACKAGE_COST_FETCH_FAILED',
					error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
				});
			}
			console.log(error);
		}
	},
	onPackageCostDestroy: () => {
		dispatch({type: 'PACKAGE_COST_DESTROY', packageCode: ownProps.params.packageCode})
	},
});

const component =  connect(mapStateToProps, mapDispatchToProps)(Package);
export const PackageConnector = Permissioned(component, getComponentViewPermissions('package'), true, null);
