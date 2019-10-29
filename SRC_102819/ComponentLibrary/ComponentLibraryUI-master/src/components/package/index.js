import React from 'react';
import styles from './index.css';
import {AutoAffix} from 'react-overlays';
import {containerName, getSaasToken, head, idFor, isFetched} from '../../helpers';
import {Loading} from '../loading';
import { button } from '../../css-common/forms.css';
import classNames from 'classnames';
import { Link } from "react-router";
import {Component} from '../component';
import {ComponentStatus} from '../component-status';
import moment from 'moment-timezone';
import {ComponentImageGallery} from "../image-gallery/index";
import {PermissionedForNonComponents} from "../../permissions/permissions";
import {getEditComponentPermissions} from "../../permissions/ComponentPermissions";

const locations = ['Bangalore', 'Hyderabad','Pune'];

export class Package extends React.Component {
	constructor(props) {
		super(props);
		this.handlePackageCostFetchRequest = this.handlePackageCostFetchRequest.bind(this);
		this.state = {
      galleryIsOpen: false,
			location: locations[0],
			date:moment(),
		}
	}

	componentDidMount() {
		if(!isFetched(this.props.details)) {
			this.props.onPackageFetchRequest(this.props.packageCode);
		}
		if(!this.props.cost) {
			this.props.onPackageCostFetchRequest();
		}
	}

	componentWillReceiveProps(nextProps){
	}

	componentWillUnmount() {
		this.props.onPackageDestroy();
	}

	renderError() {
		return <h3 id={idFor('error')}>{this.props.error}</h3>;
	}

	renderImage(details) {
    const imageValue = head(details, 'general', 'image','value');
    const imageUrl = imageValue && imageValue[0] && imageValue[0].url+'?'+getSaasToken(containerName);
		return imageUrl ? <div className={styles.headerImage}
                       onClick={e => {
                          e.preventDefault();
                          return this.setState({ galleryIsOpen: true });
                        }}
                        style={{backgroundImage: `url(${imageUrl})`}}/> : '';
	}

	handlePackageCostFetchRequest(location,date) {
		this.setState({location,date});
		this.props.onPackageCostDestroy();
		let newDate = date.utc().format();
		this.props.onPackageCostFetchRequest(location,newDate);
	}

	render() {
		if (isFetched(this.props.details) && (this.props.cost || this.props.packageCostError)){
      const classes = classNames(button, styles.add);
			const classificationDefinition = this.props.details.values.headers.find(header => header.key === 'classification_definition');
			const toBeShownDetails = {headers:this.props.details.values.headers.filter(header => header.key !== 'classification_definition')};

			console.log(classificationDefinition);
      const images = head(this.props.details && this.props.details.values,'general','image','value') || [];
			return (<div className={styles.material}>
        <ComponentImageGallery isOpen={this.state.galleryIsOpen} images={images} onCloseGallery={e => {
          e.preventDefault();
          this.setState({ galleryIsOpen: false });
        }} />
				<div className={styles.affixContainer}>
					<AutoAffix affixClassName={styles.headerAffixed}>
						<header className={styles.header}>
							<div className={styles.left}>
								{this.renderImage(toBeShownDetails)}
								<div className={styles.titleAndStatus}>
									<h2 id={idFor('title')} className={styles.title}>
										{head(toBeShownDetails,'general','short_description','value')} | {
										head(toBeShownDetails,'general','package_code','value')}</h2>
									<ComponentStatus
										value={head(toBeShownDetails,'general','pkg_status','value')}/>
								</div>
							</div>
              <PermissionedForNonComponents allowedPermissions={getEditComponentPermissions({componentType:'package'})}>
              <div className={styles.right}>
                <Link to={`/packages/${this.props.packageCode}/edit`} className={classes}>Edit</Link>
              </div>
              </PermissionedForNonComponents>
						</header>
					</AutoAffix>
				</div>
				<Component componentCode={this.props.packageCode}
				           details={toBeShownDetails}
				           componentType="package"
				           editable={false}
				           classificationDefinition={classificationDefinition}
				           composition={this.props.details.values.componentComposition}
				           cost={this.props.cost}
				           componentCostError={this.props.packageCostError}
				           onComponentCostFetchRequest={this.handlePackageCostFetchRequest}
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
