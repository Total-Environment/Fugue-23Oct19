import React from 'react';
import {Search} from '../search';
import { Link } from 'react-router';
import {floatingAction} from '../../css-common/forms.css';
import {ComponentSearchConnector} from '../../component_search_connector.js'
import styles from './index.css';
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";

export class ServiceHome extends React.Component {

  render() {
    return <div className={styles.serviceHome}>
      <ComponentSearchConnector componentType="service" isListing="true"/>
      <PermissionedForNonComponents allowedPermissions={[permissions.CREATE_SERVICE_PROPERTIES]}>
      <Link className={floatingAction} to="/services/new">+</Link>
      </PermissionedForNonComponents>
    </div>
  }
}
