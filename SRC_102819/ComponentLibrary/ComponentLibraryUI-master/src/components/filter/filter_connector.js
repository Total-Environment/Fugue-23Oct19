import { connect } from 'react-redux';
import { Filter } from './index';
import {apiUrl, logException} from '../../helpers';
import axios from 'axios';

export default class FilterComponentConnector {
    static mapStateToProps(state, props) {
        return {
            classificationData: props.classificationData || null,
            applyFilter: props.applyFilter,
            clearFilter: props.clearFilter,
            closeDialog: props.closeDialog
        };
    }
    static mapDispatchToProps(dispatch) {
        let _self = FilterComponentConnector;
        return {
            fetchMasterDataByName: async (name) => {
                try {
                    dispatch({ type: 'MASTER_DATA_FETCH_LOADING', name });
                    const response = await axios.get(apiUrl(`master-data/name/${masterDataId}`));
                    const masterData = response.data;
                    dispatch({ type: 'MASTER_DATA_FETCH_SUCCEEDED', masterData });
                } catch (error) {
                    logException(error);
                    dispatch({ type: 'MASTER_DATA_FETCH_FAILED', error });
                }
            }
        };
    }
    static GetComponent() {
        let _self = FilterComponentConnector;
        return connect(
            _self.mapStateToProps,
            _self.mapDispatchToProps
        )(Filter);
    }
}
