import React from 'react';
import {Search} from '../search';
import { Link } from 'react-router';
import {floatingAction} from '../../css-common/forms.css';
import {ComponentSearchConnector} from '../../component_search_connector.js';
import styles from './index.css';
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";

export class PackageHome extends React.Component {

  render() {
    return <div className={styles.packageHome}>
      <ComponentSearchConnector componentType="package" isListing="true"/>
      <PermissionedForNonComponents allowedPermissions={[permissions.CREATE_PACKAGE]}>
      <Link className={floatingAction} to="/packages/new">+</Link>
      </PermissionedForNonComponents>
    </div>
  }
}
