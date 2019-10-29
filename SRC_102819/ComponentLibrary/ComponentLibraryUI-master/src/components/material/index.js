import React from 'react';
import styles from './index.css';
import { Loading } from '../loading';
import {containerName, getSaasToken, head, idFor} from '../../helpers';
import { Component } from '../component';
import { button } from '../../css-common/forms.css';
import classNames from 'classnames';
import { Link } from 'react-router';
import { ComponentStatus } from '../component-status';
import { AutoAffix } from 'react-overlays';
import { ComponentImageGallery } from '../image-gallery';
import {Permissioned, PermissionedForNonComponents, permissions} from "../../permissions/permissions";
import {getEditComponentPermissions} from "../../permissions/ComponentPermissions";

export class Material extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      galleryIsOpen: false
    };
  }
  componentDidMount() {
    if (!this.props.details) {
      this.props.onMaterialFetchRequest(this.props.materialCode);
    }
    if (!this.props.rates) {
      this.props.onMaterialRatesFetchRequest(this.props.materialCode);
    }
    if (head(this.props.details, 'general', 'can_be_used_as_an_asset','value') && !this.props.rentalRates) {
      this.props.onRentalRatesFetchRequest(this.props.materialCode);
    }
  }

  componentWillReceiveProps(nextProps) {
    if (head(nextProps.details, 'general', 'can_be_used_as_an_asset','value') && !nextProps.rentalRates)
      this.props.onRentalRatesFetchRequest(this.props.materialCode);
  }

  componentWillUnmount() {
    this.props.onMaterialDestroy();
  }

  renderImage() {
    const imageValue = head(this.props.details, 'general', 'image','value');
    const imageUrl = imageValue && imageValue[0] && imageValue[0].url+'?'+getSaasToken(containerName);
    return imageUrl ? <div className={styles.headerImage}
      onClick={e => {
        e.preventDefault();
        return this.setState({ galleryIsOpen: true });
      }}
      style={{ backgroundImage: `url("${imageUrl}")` }} /> : '';
  }

  render() {
    const classes = classNames(button, styles.add);
    const images = head(this.props.details,'general','image','value') || [];
    if (this.props.details) {
      return (<div className={styles.material}>
        <ComponentImageGallery isOpen={this.state.galleryIsOpen} images={images} onCloseGallery={e => {
          e.preventDefault();
          this.setState({ galleryIsOpen: false });
        }} />
        <div className={styles.affixContainer}>
          <AutoAffix affixClassName={styles.headerAffixed}>
            <header className={styles.header}>
              <div className={styles.left}>
                {this.renderImage()}
                <div className={styles.titleAndStatus}>
                  <h2 id={idFor('title')} className={styles.title}>
                    {head(this.props.details,'general','material_name','value')} | {
                      head(this.props.details,'general', 'material_code','value')}</h2>
                  <ComponentStatus
                    value={head(this.props.details,'general','material_status','value')} />
                </div>
              </div>
              <PermissionedForNonComponents allowedPermissions={getEditComponentPermissions({componentType:'material'})}>
                <div className={styles.right}>
                  <Link to={`/materials/${this.props.materialCode}/edit`} className={classes}>Edit</Link>
                </div>
              </PermissionedForNonComponents>
            </header>
          </AutoAffix>
        </div>
        <Component componentCode={this.props.materialCode} details={this.props.details}
          rates={this.props.rates}
          rateserror={this.props.rateserror}
          rentalRates={this.props.rentalRates}
          rentalRatesError={this.props.rentalRatesError}
          rateHistoryLink={`/materials/${this.props.materialCode}/rate-history`}
          rentalRateHistoryLink={`/materials/${this.props.materialCode}/rental-rate-history`}
          componentType="material"
          editable={false}
        />

      </div>);
    }
    else if (this.props.detailserror !== '' && this.props.detailserror !== undefined) {
      return <h3 id={idFor('detailserror')}>{this.props.detailserror}</h3>;
    }
    return <Loading />;
  }
}
