import {PackageHome} from './components/package-home';
import {connect} from 'react-redux';

export const mapStateToProps = (state, props) => ({
  groups: state.reducer.packageGroupSearchDetails ? state.reducer.packageGroupSearchDetails.groups:[],
  searchResponse: state.reducer.packageGroupSearchDetails? state.reducer.packageGroupSearchDetails.response : state.reducer.error
});

export const mapDispatchToProps = (dispatch) => ({});
export const PackageHomeConnector = connect(mapStateToProps, mapDispatchToProps)(PackageHome);
