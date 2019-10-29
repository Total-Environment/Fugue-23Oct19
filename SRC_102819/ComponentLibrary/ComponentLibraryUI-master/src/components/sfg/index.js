import React from 'react';
import styles from './index.css';
import { Loading } from '../loading';
import {getHeader, head, idFor, isFetched} from '../../helpers';
import { Component } from '../component';
import { button } from '../../css-common/forms.css';
import classNames from 'classnames';
import { ComponentStatus } from '../component-status';
import { AutoAffix } from 'react-overlays';
import moment from 'moment-timezone';
import { Link } from "react-router";
import {PermissionedForNonComponents} from "../../permissions/permissions";
import {getEditComponentPermissions} from "../../permissions/ComponentPermissions";

const locations = ['Bangalore', 'Hyderabad', 'Pune'];

export class Sfg extends React.Component {
  constructor(props) {
    super(props);
    this.handleSfgCostFetchRequest = this.handleSfgCostFetchRequest.bind(this);
    this.state = {
      location: locations[0],
      date: moment(),
    }
  }

  componentDidMount() {
    if (!isFetched(this.props.details)) {
      this.props.onSfgFetchRequest(this.props.sfgCode);
    }
    if (!this.props.cost) {
      this.props.onSfgCostFetchRequest();
    }
  }

  componentWillReceiveProps(nextProps) {
  }

  componentWillUnmount() {
    this.props.onSfgDestroy();
  }

  renderImage(details) {
    const imageUrl = details && details.general && details.general.image && details.general.image.value && details.general.image.value[0] && details.general.image.value[0].url;
    return imageUrl ? <a className={styles.headerImage} target="_blank" href={imageUrl}
      style={{ backgroundImage: `url(${imageUrl})` }} /> : '';
  }


  renderError() {
    return <h3 id={idFor('error')}>{this.props.error}</h3>;
  }

  handleSfgCostFetchRequest(location, date) {
    this.setState({ location, date });
    this.props.onSfgCostDestroy();
    let newDate = date.utc().format();
    this.props.onSfgCostFetchRequest(location, newDate);
  }

  render() {
    if (this.props.sfgError && this.props.sfgError.type === 'NotFound') {
      return (<div className={styles.material}>
        <div className={styles.affixContainer}>
          <div className={styles.titleAndStatus}>
            <h2>{this.props.sfgError.message}</h2>
          </div>
        </div>
      </div>);
    }
    if (isFetched(this.props.details) && (this.props.cost || this.props.sfgCostError)) {
      const classes = classNames(button, styles.add);
      const classificationDefinition = getHeader(this.props.details.values,'classification_definition') || null;
      const headers = this.props.details.values && this.props.details.values.headers && this.props.details.values.headers.filter(header => header.key !== 'classification_definition');
      const toBeShownDetails = {headers};
      return (<div className={styles.material}>
        <div className={styles.affixContainer}>
          <AutoAffix affixClassName={styles.headerAffixed}>
            <header className={styles.header}>
              <div className={styles.left}>
                {this.renderImage(toBeShownDetails)}
                <div className={styles.titleAndStatus}>
                  <h2 id={idFor('title')} className={styles.title}>
                    {head(toBeShownDetails,'general', 'short_description', 'value')} | {
                      head(toBeShownDetails, 'general', 'sfg_code','value')}</h2>
                  <ComponentStatus
                    value={head(toBeShownDetails, 'general', 'sfg_status', 'value')} />
                </div>
              </div>
              <PermissionedForNonComponents allowedPermissions={getEditComponentPermissions({componentType:'sfg'})}>
              <div className={styles.right}>
                <Link to={`/sfgs/${this.props.sfgCode}/edit`} className={classes}>Edit</Link>
              </div>
              </PermissionedForNonComponents>
            </header>
          </AutoAffix>
        </div>
        <Component componentCode={this.props.sfgCode}
          details={toBeShownDetails}
          composition={this.props.details.values.componentComposition}
          cost={this.props.cost}
          componentCostError={this.props.sfgCostError}
          classificationDefinition={classificationDefinition}
          componentType="sfg"
          onComponentCostFetchRequest={this.handleSfgCostFetchRequest}
          editable={false}
          locations={locations}
          componentLocation={this.state.location}
          componentDate={this.state.date}
        />
      </div>);
    }
    else if (this.props.error) {
      return <div>{this.renderError()}</div>
    }
    else {
      return <Loading />;
    }
  }
}
