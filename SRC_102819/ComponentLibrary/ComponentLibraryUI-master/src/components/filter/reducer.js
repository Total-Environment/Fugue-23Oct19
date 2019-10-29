'use strict';
import R from 'ramda';
const actions = {
    MASTER_DATA_FETCH_LOADING: (action) => {
        return [{
            masterDataName: action.name 
        }];
    },
    MASTER_DATA_FETCH_SUCCEEDED: (action) => {
        return [{ masterData: action.masterData }];
    },
    MASTER_DATA_FETCH_FAILED: (action) => {
        return [{
            error: action.error
        }];
    }
};

const fetchAction = (actionType) => {
    return (action) => {
        return (
            actions[actionType] || (() => [{}])
        )(action);
    };
}

const filterCompReducer = (state = {}, action) => {
    let data = Object.assign(
        {},
        state,
        R.mergeAll(
            fetchAction(action.type)(action)
        ));
    return data;
};

export default filterCompReducer;