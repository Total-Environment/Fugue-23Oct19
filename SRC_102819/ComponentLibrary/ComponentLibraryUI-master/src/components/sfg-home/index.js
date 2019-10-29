import {floatingAction} from '../../css-common/forms.css';
import * as React from "react";
import {Link} from "react-router";
import styles from './index.css';
import {ComponentSearchConnector} from '../../component_search_connector.js'
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";

export class SfgHome extends React.Component {

  render() {
    return <div className={styles.sfgHome}>
      <ComponentSearchConnector componentType="SFG" isListing="true"/>
      <PermissionedForNonComponents allowedPermissions={[permissions.CREATE_SFG]}>
      <Link className={floatingAction} to="/sfgs/new">+</Link>
      </PermissionedForNonComponents>
    </div>
  }
}
