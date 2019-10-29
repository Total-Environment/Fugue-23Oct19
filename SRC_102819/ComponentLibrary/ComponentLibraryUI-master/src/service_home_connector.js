import {ServiceHome} from './components/service-home';
import {connect} from 'react-redux';

export const mapStateToProps = (state, props) => ({
    groups: state.reducer.serviceGroupSearchDetails ? state.reducer.serviceGroupSearchDetails.groups:[],
    searchResponse: state.reducer.serviceGroupSearchDetails? state.reducer.serviceGroupSearchDetails.response : state.reducer.error
});

export const mapDispatchToProps = (dispatch) => ({});
export const ServiceHomeConnector = connect(mapStateToProps, mapDispatchToProps)(ServiceHome);
