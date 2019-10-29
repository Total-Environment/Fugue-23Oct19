import {MaterialHome} from './components/material-home';
import {connect} from 'react-redux';

export const mapStateToProps = (state, props) => ({
    groups: state.reducer.materialGroupSearchDetails ? state.reducer.materialGroupSearchDetails.groups:[],
    searchResponse: state.reducer.materialGroupSearchDetails? state.reducer.materialGroupSearchDetails.response : state.reducer.error
});

export const mapDispatchToProps = (dispatch) => ({});
export const MaterialHomeConnector = connect(mapStateToProps, mapDispatchToProps)(MaterialHome);
