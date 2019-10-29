import {PriceBook} from "./index";
import {connect} from "react-redux";
import {fetchCprs, fetchDependencyDefinitionIfNeeded, fetchProjectsIfNeeded} from "../../actions";
import {bindActionCreators} from "redux";
import {Permissioned, permissions} from "../../permissions/permissions";
import {getViewCPRValuesPermissions} from "../../permissions/ComponentPermissions";

export const mapStateToProps = (state) => ({
  cprs: state.reducer.cprs,
  dependencyDefinitions: state.reducer.dependencyDefinitions,
  projects: state.reducer.projects,
  dependencyDefinitionError: state.reducer.dependencyDefinitionError,
  cprError: state.reducer.cprError,
  projectsError: state.reducer.projectsError,

});
export const mapDispatchToProps = (dispatch) => ({
  onPriceBookDestroy: () => {
    dispatch({type:'DESTROY_PRICEBOOK_DETAILS'})
  },
  ...bindActionCreators({fetchDependencyDefinitionIfNeeded, fetchCprs, fetchProjectsIfNeeded}, dispatch),
});

const component = connect(mapStateToProps, mapDispatchToProps)(PriceBook);
export const PriceBookConnector = Permissioned(component, getViewCPRValuesPermissions, true,null);
