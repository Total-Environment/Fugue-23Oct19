import React from 'react';
import {Search} from '../search';
import { Link } from 'react-router';
import {floatingAction} from '../../css-common/forms.css';
import {ComponentSearchConnector} from '../../component_search_connector.js';
import styles from './index.css';
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";

export class MaterialHome extends React.Component {

  render() {
    return <div className={styles.materialHome}>
      <ComponentSearchConnector componentType="material" isListing="true"/>
      <PermissionedForNonComponents allowedPermissions={[permissions.CREATE_MATERIAL_PROPERTIES]}>
      <Link className={floatingAction} to="/materials/new">+</Link>
      </PermissionedForNonComponents>,
    </div>
  }

  getPermissions(componentType) {
    switch (componentType) {
      case 'material':
        return [permissions.CREATE_MATERIAL_PROPERTIES];
      case 'service':
        return [permissions.CREATE_SERVICE_PROPERTIES];
      default:
        return [];
    }
  }
}
