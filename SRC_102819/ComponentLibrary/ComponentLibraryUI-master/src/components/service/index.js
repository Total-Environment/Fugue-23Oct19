import React from 'react';
import {Component} from '../component'
import {Loading} from '../loading';
import {head, idFor} from '../../helpers'
import styles from './index.css'
import {ComponentStatus} from '../component-status';
import {AutoAffix} from 'react-overlays';
import {Link} from 'react-router';
import {button} from '../../css-common/forms.css';
import classNames from 'classnames';
import {PermissionedForNonComponents} from "../../permissions/permissions";
import {getEditComponentPermissions} from "../../permissions/ComponentPermissions";

export class Service extends React.Component {
  componentDidMount() {
    if (!this.props.details) {
      this.props.onServiceFetchRequest(this.props.serviceCode);
    }
    if (!this.props.rates) {
      this.props.onServiceRatesFetchRequest(this.props.serviceCode);
    }
  }

  componentWillUnmount() {
    this.props.onServiceDestroy();
  }

  render() {
    const classes = classNames(button, styles.add);
    if (this.props.details) {
      return (<div className={styles.service}>
        <div className={styles.affixContainer}>
          <AutoAffix affixClassName={styles.headerAffixed}>
            <header className={styles.header}>
              <div className={styles.left}>
                <div className={styles.titleAndStatus}>
              <h2 id={idFor('title')} className={styles.title}>
                {head(this.props.details,'general','service_code','value')}
              </h2>
              <ComponentStatus
                value={head(this.props.details,'general','service_status','value')}/>
              </div>
              </div>
              <PermissionedForNonComponents allowedPermissions={getEditComponentPermissions({componentType:'service'})}>
              <div className={styles.right}>
                <Link to={`/services/${this.props.serviceCode}/edit`} className={classes}>Edit</Link>
              </div>
              </PermissionedForNonComponents>
            </header>
          </AutoAffix>
        </div>
        <Component code={this.props.serviceCode}
                   componentCode={this.props.serviceCode}
                   details={this.props.details}
                   rates={this.props.rates}
                   rateserror={this.props.rateserror}
                   rateHistoryLink={`/services/${this.props.serviceCode}/rate-history`}
                   classificationDefinition={this.props.classificationDefinition}
                   componentType="service"
                   editable={false}
        />
      </div>);
    }
    else if (this.props.error !== '' && this.props.error !== undefined) {
      return <h3 id={idFor('error')}>{this.props.error}</h3>;
    }
    return <Loading/>;
  }
}
